using Kogi.Scripts.Combat;
using UnityEngine;

namespace Kogi.Scripts.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class DaggerProjectile : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float speed = 10f;

        [SerializeField, Min(0.1f)]
        private float lifetime = 2.5f;

        [SerializeField, Min(1)]
        private int damage = 1;

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

            if (other.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damage);
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