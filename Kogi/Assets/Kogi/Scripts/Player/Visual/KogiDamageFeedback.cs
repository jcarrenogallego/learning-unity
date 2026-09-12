using System.Collections;
using UnityEngine;

namespace Kogi.Scripts.Player.Visual
{
    public sealed class KogiDamageFeedback : MonoBehaviour
    {
        [SerializeField] private KogiDamageReceiver damageReceiver;
        [SerializeField] private SpriteRenderer[] renderers;
        [SerializeField, Min(0.05f)] private float duration = 0.35f;

        private void OnEnable()
        {
            damageReceiver.HitReceived += Play;
        }

        private void OnDisable()
        {
            damageReceiver.HitReceived -= Play;
        }

        private void Play()
        {
            StopAllCoroutines();
            StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            float elapsed = 0f;
            Color hitColor = new Color(1f, 0.35f, 0.3f, 1f);

            while (elapsed < duration)
            {
                float blend = Mathf.PingPong(elapsed * 12f, 1f);
                foreach (SpriteRenderer item in renderers)
                {
                    item.color = Color.Lerp(Color.white, hitColor, blend);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            foreach (SpriteRenderer item in renderers)
            {
                item.color = Color.white;
            }
        }
    }
}
