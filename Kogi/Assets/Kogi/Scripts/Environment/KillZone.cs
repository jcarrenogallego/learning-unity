using Kogi.Scripts.Player;
using UnityEngine;

namespace Kogi.Scripts.Environment
{
    public sealed class KillZone : MonoBehaviour
    {
        [SerializeField]
        private Transform respawnPoint;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out KogiLives lives))
            {
                return;
            }

            lives.LoseLife(respawnPoint.position);
        }
    }
}
