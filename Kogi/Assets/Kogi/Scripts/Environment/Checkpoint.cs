using Kogi.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kogi.Scripts.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        private string checkpointId = "desierto-centro";

        [SerializeField]
        private Transform respawnPosition;

        private bool isActive;

        public void Configure(string identifier, Transform position)
        {
            checkpointId = identifier;
            respawnPosition = position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out KogiLives _))
            {
                return;
            }

            Vector2 position = respawnPosition != null ? respawnPosition.position : transform.position;
            CheckpointState.Activate(SceneManager.GetActiveScene().name, checkpointId, position);
            isActive = true;
            Debug.Log($"Checkpoint activado: {checkpointId}");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isActive ? Color.cyan : new Color(0.3f, 0.7f, 1f, 0.7f);
            Gizmos.DrawWireSphere(respawnPosition != null ? respawnPosition.position : transform.position, 0.35f);
        }
    }
}
