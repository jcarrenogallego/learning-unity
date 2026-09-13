using Kogi.Scripts.Player;
using UnityEngine;

namespace Kogi.Scripts.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Collectible : MonoBehaviour
    {
        [SerializeField]
        private string collectibleId = "desert-relic-01";

        public void Configure(string identifier)
        {
            collectibleId = identifier;
        }

        private void Start()
        {
            if (CollectibleProgress.Contains(collectibleId))
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            transform.Rotate(0f, 0f, 45f * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out KogiLives _) || !CollectibleProgress.Collect(collectibleId))
            {
                return;
            }

            Debug.Log($"Reliquia recogida: {collectibleId}");
            Destroy(gameObject);
        }
    }
}
