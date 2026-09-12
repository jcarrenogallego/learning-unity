using System;
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
        private Transform characterVisual;

        [SerializeField, Min(0.1f)]
        private float invulnerabilityDuration = 1.5f;

        [SerializeField, Min(0.05f)]
        private float flashInterval = 0.1f;

        private KogiLives lives;
        private bool isInvulnerable;

        public event Action HitReceived;

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
            HitReceived?.Invoke();
            lives.LoseLife(respawnPoint.position);
            StartCoroutine(ShowInvulnerability());
        }

        private IEnumerator ShowInvulnerability()
        {
            float elapsedTime = 0f;

            while (elapsedTime < invulnerabilityDuration)
            {
                characterVisual.gameObject.SetActive(!characterVisual.gameObject.activeSelf);
                yield return new WaitForSeconds(flashInterval);
                elapsedTime += flashInterval;
            }

            characterVisual.gameObject.SetActive(true);
            isInvulnerable = false;
        }
    }
}
