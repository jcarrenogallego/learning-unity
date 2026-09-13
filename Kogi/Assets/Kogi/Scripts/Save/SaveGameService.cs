using System;
using System.IO;
using Kogi.Scripts.Environment;
using Kogi.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Kogi.Scripts.UI;

namespace Kogi.Scripts.Save
{
    public sealed class SaveGameService : MonoBehaviour
    {
        [Serializable]
        private sealed class SaveData
        {
            public string sceneName;
            public string checkpointId;
            public float positionX;
            public float positionY;
            public string[] collectibles;
        }

        public static SaveGameService Instance { get; private set; }
        internal string VerificationSavePath { get; set; }
        public string SavePath => VerificationSavePath ?? Path.Combine(Application.persistentDataPath, "kogi-save.json");
        public bool HasSave => File.Exists(SavePath);

        private Vector2? pendingSpawnPosition;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (Instance != null)
            {
                return;
            }

            GameObject service = new GameObject("SaveGameService");
            DontDestroyOnLoad(service);
            Instance = service.AddComponent<SaveGameService>();
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
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void Update()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                Save();
            }
            else if (Keyboard.current.f9Key.wasPressedThisFrame)
            {
                Load();
            }
        }

        public void Save()
        {
            if (GameFlowController.Instance != null && GameFlowController.Instance.IsFinished) return;
            Scene scene = SceneManager.GetActiveScene();
            CheckpointState.TryGet(scene.name, out string checkpointId, out Vector2 position);

            if (string.IsNullOrEmpty(checkpointId))
            {
                KogiRespawn kogi = FindAnyObjectByType<KogiRespawn>();
                position = kogi != null ? kogi.transform.position : Vector2.zero;
            }

            SaveData data = new SaveData
            {
                sceneName = scene.name,
                checkpointId = checkpointId,
                positionX = position.x,
                positionY = position.y,
                collectibles = CollectibleProgress.GetIds()
            };

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"Partida guardada en {SavePath}");
        }

        public void Load()
        {
            if (!HasSave)
            {
                Debug.Log("Todavía no existe una partida guardada.");
                return;
            }

            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            CollectibleProgress.Restore(data.collectibles);
            pendingSpawnPosition = new Vector2(data.positionX, data.positionY);
            CheckpointState.Activate(data.sceneName, data.checkpointId, pendingSpawnPosition.Value);
            if (GameFlowController.Instance != null) GameFlowController.Instance.ResumeForSceneLoad();
            Time.timeScale = 1f;
            AudioListener.pause = false;
            SceneManager.LoadScene(data.sceneName);
        }

        public void DeleteSave()
        {
            if (HasSave)
            {
                File.Delete(SavePath);
            }

            CheckpointState.Clear();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!pendingSpawnPosition.HasValue)
            {
                return;
            }

            KogiRespawn kogi = FindAnyObjectByType<KogiRespawn>();
            if (kogi != null) kogi.RespawnAt(pendingSpawnPosition.Value);
            pendingSpawnPosition = null;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
    }
}
