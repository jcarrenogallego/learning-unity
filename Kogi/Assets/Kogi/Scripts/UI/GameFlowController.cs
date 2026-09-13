using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Kogi.Scripts.Environment;

namespace Kogi.Scripts.UI
{
    public sealed class GameFlowController : MonoBehaviour
    {
        public static GameFlowController Instance { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsVictory { get; private set; }
        public bool IsFinished => IsGameOver || IsVictory;
        private int finishedFrame;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (Instance != null)
            {
                return;
            }

            GameObject controller = new GameObject("GameFlowController");
            DontDestroyOnLoad(controller);
            Instance = controller.AddComponent<GameFlowController>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!IsFinished || Time.frameCount == finishedFrame)
            {
                return;
            }

            bool restart = Keyboard.current != null &&
                (Keyboard.current.rKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);

            if (restart)
            {
                if (IsVictory) RestartPrototype();
                else RestartCurrentScene();
            }
        }

        public void ShowGameOver()
        {
            if (IsFinished)
            {
                return;
            }

            IsGameOver = true;
            FreezeFinishedGame();
        }

        public void ShowVictory()
        {
            if (IsFinished) return;
            IsVictory = true;
            FreezeFinishedGame();
        }

        private void FreezeFinishedGame()
        {
            finishedFrame = Time.frameCount;
            if (PauseController.Instance != null) PauseController.Instance.ClearPause();
            foreach (PlayerInput input in FindObjectsByType<PlayerInput>())
            {
                input.DeactivateInput();
            }
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        public void ResumeForSceneLoad()
        {
            IsGameOver = false;
            IsVictory = false;
            if (PauseController.Instance != null) PauseController.Instance.ClearPause();
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        public void RestartCurrentScene()
        {
            ResumeForSceneLoad();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void RestartPrototype()
        {
            ResumeForSceneLoad();
            CheckpointState.Clear();
            CollectibleProgress.Restore(null);
            SceneManager.LoadScene("NivelDesierto");
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null;
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        private void OnGUI()
        {
            if (!IsFinished)
            {
                return;
            }

            Rect panel = new Rect((Screen.width - 500f) * 0.5f, (Screen.height - 280f) * 0.5f, 500f, 280f);
            GUI.Box(panel, string.Empty);
            var titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter };
            var textStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, wordWrap = true };
            GUI.Label(new Rect(panel.x + 15f, panel.y + 25f, 470f, 40f), IsVictory ? "¡SANTUARIO COMPLETADO!" : "FIN DE PARTIDA", titleStyle);
            GUI.Label(new Rect(panel.x + 25f, panel.y + 75f, 450f, 55f), IsVictory
                ? "Has derrotado al Guardián del Desierto.\nFin del prototipo. La aventura de Kogi continuará."
                : "Kogi se ha quedado sin vidas", textStyle);

            if (GUI.Button(new Rect(panel.x + 125f, panel.y + 145f, 250f, 45f), IsVictory ? "Jugar de nuevo (R)" : "Reintentar (R)"))
            {
                if (IsVictory) RestartPrototype();
                else RestartCurrentScene();
            }

            if (!Application.isEditor && GUI.Button(new Rect(panel.x + 125f, panel.y + 205f, 250f, 40f), "Salir")) Application.Quit();
        }
    }
}
