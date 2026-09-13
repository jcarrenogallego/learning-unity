using System;
using UnityEngine;
using Kogi.Scripts.UI;
using Kogi.Scripts.Environment;
using UnityEngine.SceneManagement;

namespace Kogi.Scripts.Player
{
    [RequireComponent(typeof(KogiRespawn))]
    public sealed class KogiLives : MonoBehaviour
    {
        public event Action<int> LivesChanged;

        public int CurrentLives => currentLives;

        [SerializeField, Min(1)]
        private int startingLives = 3;

        private KogiRespawn respawn;
        private int currentLives;

        private void Awake()
        {
            respawn = GetComponent<KogiRespawn>();
            currentLives = startingLives;
        }

        public void LoseLife(Vector2 respawnPosition)
        {
            if (currentLives <= 0 || (GameFlowController.Instance != null && GameFlowController.Instance.IsFinished)) return;
            currentLives--;
            LivesChanged?.Invoke(currentLives);
            Debug.Log($"Vidas restantes: {currentLives}");

            if (currentLives <= 0)
            {
                GameFlowController.Instance.ShowGameOver();
                return;
            }

            string sceneName = SceneManager.GetActiveScene().name;
            Vector2 safePosition = CheckpointState.GetRespawnPosition(sceneName, respawnPosition);
            respawn.RespawnAt(safePosition);
        }
    }
}
