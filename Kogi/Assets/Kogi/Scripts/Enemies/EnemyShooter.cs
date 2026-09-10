using Kogi.Scripts.Player;
using Kogi.Scripts.Projectiles;
using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    public sealed class EnemyShooter : MonoBehaviour
    {
        [SerializeField]
        private Transform firePoint;

        [SerializeField]
        private EnemyProjectile projectilePrefab;

        [SerializeField, Min(0f)]
        private float detectionRange = 6f;

        [SerializeField, Min(0.1f)]
        private float shotCooldown = 2f;

        private Transform target;
        private float remainingCooldown;

        private void Start()
        {
            KogiDamageReceiver receiver = FindFirstObjectByType<KogiDamageReceiver>();

            if (receiver is not null)
            {
                target = receiver.transform;
            }
        }

        private void Update()
        {
            remainingCooldown -= Time.deltaTime;

            if (target is null || remainingCooldown > 0f)
            {
                return;
            }

            Vector2 direction = target.position - firePoint.position;

            if (direction.magnitude > detectionRange)
            {
                return;
            }

            Shoot(direction);
            remainingCooldown = shotCooldown;
        }

        private void Shoot(Vector2 direction)
        {
            Vector3 firePointPosition = firePoint.localPosition;
            firePointPosition.x = Mathf.Abs(firePointPosition.x)
                * Mathf.Sign(direction.x);
            firePoint.localPosition = firePointPosition;

            EnemyProjectile projectile = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity);

            projectile.Launch(direction, gameObject);
        }
    }
}