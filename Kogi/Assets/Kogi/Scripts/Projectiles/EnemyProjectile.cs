using Kogi.Scripts.Player;
using UnityEngine;

namespace Kogi.Scripts.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class EnemyProjectile : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float speed = 7f;

        [SerializeField, Min(0.1f)]
        private float lifetime = 3f;

        private Rigidbody2D body;
        private GameObject owner;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 direction, GameObject projectileOwner)
        {
            owner = projectileOwner;
            Vector2 normalizedDirection = direction.normalized;
            body.linearVelocity = normalizedDirection * speed;
            transform.right = normalizedDirection;
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == owner)
            {
                return;
            }

            if (other.TryGetComponent(out KogiDamageReceiver receiver))
            {
                receiver.ReceiveHit();
                Destroy(gameObject);
                return;
            }

            if (!other.isTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}