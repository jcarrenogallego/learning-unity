using System;
using UnityEngine;
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
            currentLives--;
            LivesChanged?.Invoke(currentLives);
            Debug.Log($"Vidas restantes: {currentLives}");

            if (currentLives <= 0)
            {
                Scene activeScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(activeScene.buildIndex);
                return;
            }

            respawn.RespawnAt(respawnPosition);
        }
    }
}
