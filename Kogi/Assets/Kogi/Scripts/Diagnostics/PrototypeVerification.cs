#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kogi.Scripts.Audio;
using Kogi.Scripts.Combat;
using Kogi.Scripts.Enemies;
using Kogi.Scripts.Environment;
using Kogi.Scripts.Interaction;
using Kogi.Scripts.Player;
using Kogi.Scripts.Projectiles;
using Kogi.Scripts.Save;
using Kogi.Scripts.UI;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

namespace Kogi.Scripts.Diagnostics
{
    // Development-only, opt-in checks. Never runs in a normal game session.
    public sealed class PrototypeVerification : MonoBehaviour
    {
        [Serializable]
        public sealed class Report
        {
            public bool passed;
            public string environment;
            public List<string> checks = new List<string>();
            public List<string> errors = new List<string>();
            public float sampleSeconds;
            public float averageFps;
            public float p95FrameMs;
            public long memoryStartBytes;
            public long memoryEndBytes;
            public long memoryPeakBytes;
            public double mainThreadAverageMs;
        }

        private readonly Report report = new Report();
        private string outputDirectory;
        private bool quitWhenDone;
        private Keyboard keyboard;
        private Mouse mouse;
        private ProfilerRecorder memory;
        private ProfilerRecorder mainThread;
        private readonly List<GameObject> isolatedEnemies = new List<GameObject>();
        private bool finished;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartFromCommandLine()
        {
            string[] arguments = System.Environment.GetCommandLineArgs();
            int index = Array.IndexOf(arguments, "-kogi-verify");
            if (index >= 0 && index + 1 < arguments.Length) Begin(arguments[index + 1], true);
        }

        public static void Begin(string directory, bool quit)
        {
            if (FindAnyObjectByType<PrototypeVerification>() != null) throw new InvalidOperationException("Verification already running.");
            var go = new GameObject("PrototypeVerification");
            DontDestroyOnLoad(go);
            var runner = go.AddComponent<PrototypeVerification>();
            runner.outputDirectory = Path.GetFullPath(directory);
            runner.quitWhenDone = quit;
            Directory.CreateDirectory(runner.outputDirectory);
            runner.report.environment = Application.isEditor ? "Unity Editor" : "Windows Development Player";
            Application.logMessageReceived += runner.CaptureError;
            runner.StartCoroutine(runner.RunGuarded());
        }

        private IEnumerator RunGuarded()
        {
            var stack = new Stack<IEnumerator>();
            stack.Push(Verify());
            while (stack.Count > 0)
            {
                object current = null;
                bool moved = false;
                try
                {
                    moved = stack.Peek().MoveNext();
                    if (moved) current = stack.Peek().Current;
                }
                catch (Exception exception)
                {
                    report.errors.Add(exception.ToString());
                    break;
                }
                if (!moved) { stack.Pop(); continue; }
                if (current is IEnumerator nested) stack.Push(nested);
                else yield return current;
            }
            Finish();
        }

        private IEnumerator Verify()
        {
            yield return null;
            keyboard = InputSystem.AddDevice<Keyboard>("KogiVerificationKeyboard");
            mouse = InputSystem.AddDevice<Mouse>("KogiVerificationMouse");
            GameFlowController.Instance.RestartPrototype();
            yield return null;
            IsolateEnemies();
            PairInput();
            SaveGameService.Instance.VerificationSavePath = Path.Combine(outputDirectory, "test-save.json");
            yield return Wait(0.5f);
            Check(SceneManager.GetActiveScene().name == "NivelDesierto", "Initial scene: NivelDesierto");
            Check(Player.GetComponent<KogiLives>().CurrentLives == 3, "Initial lives: 3");
            Check(Player.GetComponent<KogiMovement>().IsGrounded, "Gravity and ground collision");
            Check(GameAudioManager.Instance.GetComponent<AudioSource>().isPlaying, "Ambient audio playing");
            Check(Player.GetComponentInChildren<Animator>() != null, "Animator connected");
            Check(FindObjectsByType<ParticleSystem>().Length > 0, "Ambient particles present");

            float x = Player.transform.position.x;
            yield return Press(Key.D, 0.25f);
            Check(Player.transform.position.x > x + 0.5f, "Keyboard horizontal movement");
            float y = Player.transform.position.y;
            yield return Press(Key.Space, 0.12f);
            Check(Player.transform.position.y > y + 0.2f, "Keyboard jump");
            yield return Wait(1.2f);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.C));
            yield return Wait(0.1f);
            Check(Player.GetComponent<KogiCrouch>().IsCrouching, "Crouch pressed");
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return Wait(0.1f);
            Check(!Player.GetComponent<KogiCrouch>().IsCrouching, "Crouch released");
            int attacks = 0;
            Player.GetComponent<KogiAttack>().AttackPerformed += () => attacks++;
            yield return Press(Key.Enter, 0.05f);
            Check(attacks == 1, "Melee attack input");
            yield return Press(Key.Q, 0.03f);
            Check(FindObjectsByType<DaggerProjectile>().Length > 0, "Dagger input and instantiation");

            PauseController.Instance.SetPaused(true);
            float frozenTime = Time.time;
            yield return Wait(0.2f);
            Check(Time.time == frozenTime && AudioListener.pause, "Pause freezes simulation and audio");
            PauseController.Instance.SetPaused(false);
            Check(Time.timeScale == 1f && !AudioListener.pause, "Resume restores simulation and audio");

            var collectible = FindObjectsByType<Collectible>().First();
            Player.GetComponent<KogiRespawn>().RespawnAt(collectible.transform.position);
            yield return Wait(0.15f);
            Check(CollectibleProgress.Count == 1, "Collectible trigger and counter");
            var checkpoint = FindAnyObjectByType<Checkpoint>();
            Player.GetComponent<KogiRespawn>().RespawnAt(checkpoint.transform.position);
            yield return Wait(0.15f);
            Check(CheckpointState.TryGet("NivelDesierto", out _, out Vector2 respawn), "Checkpoint trigger");
            int lives = Player.GetComponent<KogiLives>().CurrentLives;
            Player.GetComponent<KogiDamageReceiver>().ReceiveHit();
            Player.GetComponent<KogiDamageReceiver>().ReceiveHit();
            Check(Player.GetComponent<KogiLives>().CurrentLives == lives - 1, "Damage and temporary invulnerability");
            Check(Vector2.Distance(Player.GetComponent<Rigidbody2D>().position, respawn) < 0.1f, "Respawn uses checkpoint");
            string[] savedCollectibles = CollectibleProgress.GetIds();
            SaveGameService.Instance.Save();
            Check(File.Exists(SaveGameService.Instance.SavePath), "Save uses isolated test file");
            GameFlowController.Instance.ShowGameOver();
            PauseController.Instance.SetPaused(false);
            Check(GameFlowController.Instance.IsGameOver && Time.timeScale == 0f, "Game over cannot be dismissed by pause");
            SaveGameService.Instance.Load();
            yield return null;
            IsolateEnemies();
            PairInput();
            Check(!GameFlowController.Instance.IsFinished && Time.timeScale == 1f, "Loading clears game-over state");
            Check(new HashSet<string>(savedCollectibles).SetEquals(CollectibleProgress.GetIds()), "Loading restores collectibles");
            Check(Vector2.Distance(Player.GetComponent<Rigidbody2D>().position, respawn) < 0.2f, "Loading restores checkpoint position");

            var portal = FindAnyObjectByType<ScenePortal>();
            Player.GetComponent<KogiRespawn>().RespawnAt(portal.transform.position);
            yield return WaitForScene("SantuarioPrueba");
            IsolateEnemies();
            PairInput();
            Check(FindObjectsByType<GameFlowController>().Length == 1, "One persistent game-flow controller across scenes");
            var lever = FindAnyObjectByType<Lever>();
            var door = FindAnyObjectByType<DoorController>();
            Player.GetComponent<KogiRespawn>().RespawnAt(lever.transform.position + Vector3.left * 0.5f);
            yield return Wait(0.1f);
            yield return Press(Key.E, 0.8f);
            Check(door.IsOpen, "Hold E operates lever (release must not toggle twice)");
            yield return Wait(1.1f);
            Check(door.transform.position.y > 2.9f, "Door opens clear of route");
            var npc = FindAnyObjectByType<NpcDialogue>();
            npc.Interact();
            yield return null;
            npc.Interact();
            Check(true, "NPC dialogue opens and closes without errors");

            // Directed performance scenario: enemies restored, player held above contact
            // range, camera visits the level; projectiles are exercised without death resets.
            foreach (var enemy in isolatedEnemies) if (enemy != null) enemy.SetActive(true);
            isolatedEnemies.Clear();
            var playerBody = Player.GetComponent<Rigidbody2D>();
            Player.GetComponent<KogiMovement>().enabled = false;
            playerBody.simulated = false;
            Player.transform.position = new Vector3(-5f, 5f, 0f);
            yield return Wait(2f);
            yield return MeasurePerformance();
            Player.GetComponent<KogiMovement>().enabled = true;
            playerBody.simulated = true;
            IsolateEnemies();

            // Check the saved boss configuration, its phase transition and actual death.
            var boss = isolatedEnemies.First(go => go != null && go.GetComponent<DesertWarden>() != null);
            boss.SetActive(true);
            var health = boss.GetComponent<EnemyHealth>();
            Check(health.MaximumHealth == 12 && boss.GetComponent<EnemyVariant>() == null, "Boss configuration: 12 HP, no competing variant");
            health.TakeDamage(6);
            yield return null;
            Check(boss.GetComponent<SpriteRenderer>().color.r > 0.85f, "Boss second phase at 50 percent");
            health.TakeDamage(6);
            Check(boss.GetComponentsInChildren<Collider2D>(true).All(c => !c.enabled), "Boss stops blocking immediately");
            yield return Wait(0.7f);
            Check(boss == null && GameFlowController.Instance.IsVictory, "Boss removal and prototype victory screen");
            PauseController.Instance.SetPaused(false);
            Check(Time.timeScale == 0f, "Victory cannot be bypassed by Escape");
            ScreenCapture.CaptureScreenshot(Path.Combine(outputDirectory, "victory.png"));
            yield return Wait(0.2f);
            GameFlowController.Instance.RestartPrototype();
            yield return null;
            IsolateEnemies();
            PairInput();
            Check(SceneManager.GetActiveScene().name == "NivelDesierto" && !GameFlowController.Instance.IsFinished, "Replay returns to initial level");
            Check(Player.GetComponent<KogiLives>().CurrentLives == 3 && CollectibleProgress.Count == 0, "Replay resets lives and session progress");
            Check(File.Exists(SaveGameService.Instance.SavePath), "Replay preserves the saved file");
            Check(true, "Directed prototype integration sequence completed");
        }

        private IEnumerator MeasurePerformance()
        {
            memory = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory", 1);
            mainThread = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 1);
            var frames = new List<float>();
            long threadNanoseconds = 0;
            float started = Time.realtimeSinceStartup;
            float nextShot = started;
            while (Time.realtimeSinceStartup - started < 30f)
            {
                Player.transform.position = new Vector3(Mathf.PingPong((Time.realtimeSinceStartup - started) * 1.2f, 20f) - 10f, 5f, 0f);
                if (Time.realtimeSinceStartup >= nextShot)
                {
                    foreach (var shooter in FindObjectsByType<EnemyShooter>())
                        if (shooter.IsReady) shooter.Shoot(Vector2.up);
                    nextShot = Time.realtimeSinceStartup + 2f;
                }
                yield return null;
                frames.Add(Time.unscaledDeltaTime * 1000f);
                long used = memory.Valid ? memory.LastValue : UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
                if (frames.Count == 1) report.memoryStartBytes = used;
                report.memoryEndBytes = used;
                report.memoryPeakBytes = Math.Max(report.memoryPeakBytes, used);
                if (mainThread.Valid) threadNanoseconds += mainThread.LastValue;
            }
            report.sampleSeconds = Time.realtimeSinceStartup - started;
            report.averageFps = frames.Count / report.sampleSeconds;
            frames.Sort();
            report.p95FrameMs = frames[Mathf.Min(frames.Count - 1, Mathf.FloorToInt(frames.Count * 0.95f))];
            report.mainThreadAverageMs = mainThread.Valid ? threadNanoseconds / (double)frames.Count / 1000000d : -1;
            memory.Dispose();
            mainThread.Dispose();
            Check(true, "30-second profiler sample recorded (directed scenario, not a full performance guarantee)");
        }

        private GameObject Player => GameObject.Find("Kogi");
        private void PairInput() => Player.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard&Mouse", keyboard, mouse);
        private void IsolateEnemies()
        {
            isolatedEnemies.Clear();
            foreach (var enemy in FindObjectsByType<EnemyHealth>())
            {
                isolatedEnemies.Add(enemy.gameObject);
                enemy.gameObject.SetActive(false);
            }
        }
        private IEnumerator Press(Key key, float duration)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
            yield return Wait(duration);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return Wait(0.03f);
        }
        private static IEnumerator Wait(float seconds)
        {
            float deadline = Time.realtimeSinceStartup + seconds;
            while (Time.realtimeSinceStartup < deadline) yield return null;
        }
        private IEnumerator WaitForScene(string scene)
        {
            float deadline = Time.realtimeSinceStartup + 5f;
            while (SceneManager.GetActiveScene().name != scene && Time.realtimeSinceStartup < deadline) yield return null;
            yield return null;
            Check(SceneManager.GetActiveScene().name == scene, "Portal transition: " + scene);
        }
        private void Check(bool condition, string description)
        {
            if (!condition) throw new InvalidOperationException("FAILED: " + description);
            report.checks.Add(description);
            Debug.Log("Kogi verification: " + description);
        }
        private void CaptureError(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) report.errors.Add(message + "\n" + stack);
        }
        private void Finish()
        {
            finished = true;
            Application.logMessageReceived -= CaptureError;
            if (memory.Valid) memory.Dispose();
            if (mainThread.Valid) mainThread.Dispose();
            if (keyboard != null) InputSystem.RemoveDevice(keyboard);
            if (mouse != null) InputSystem.RemoveDevice(mouse);
            if (SaveGameService.Instance != null) SaveGameService.Instance.VerificationSavePath = null;
            report.passed = report.errors.Count == 0;
            File.WriteAllText(Path.Combine(outputDirectory, "verification.json"), JsonUtility.ToJson(report, true));
            Debug.Log("Kogi verification finished: " + report.passed);
            if (quitWhenDone) Application.Quit(report.passed ? 0 : 1);
        }
        private void OnDestroy()
        {
            Application.logMessageReceived -= CaptureError;
            if (!finished) Finish();
        }
    }
}
#endif
