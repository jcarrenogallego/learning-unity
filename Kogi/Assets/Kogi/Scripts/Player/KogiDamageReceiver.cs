using System.Collections;
using UnityEngine;

namespace Kogi.Scripts.Player
{
    [RequireComponent(typeof(KogiLives))]
    public sealed class KogiDamageReceiver : MonoBehaviour
    {
        [SerializeField]
        private Transform respawnPoint;

        [SerializeField]
        private SpriteRenderer characterRenderer;

        [SerializeField, Min(0.1f)]
        private float invulnerabilityDuration = 1.5f;

        [SerializeField, Min(0.05f)]
        private float flashInterval = 0.1f;

        private KogiLives lives;
        private bool isInvulnerable;

        private void Awake()
        {
            lives = GetComponent<KogiLives>();
        }

        public void ReceiveHit()
        {
            if (isInvulnerable)
            {
                return;
            }

            isInvulnerable = true;
            lives.LoseLife(respawnPoint.position);
            StartCoroutine(ShowInvulnerability());
        }

        private IEnumerator ShowInvulnerability()
        {
            float elapsedTime = 0f;

            while (elapsedTime < invulnerabilityDuration)
            {
                characterRenderer.enabled = !characterRenderer.enabled;
                yield return new WaitForSeconds(flashInterval);
                elapsedTime += flashInterval;
            }

            characterRenderer.enabled = true;
            isInvulnerable = false;
        }
    }
}