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

        [SerializeField, Min(0.1f)]
        private float shotCooldown = 2f;

        private float nextShotTime;

        public bool IsReady => Time.time >= nextShotTime;

        public void Shoot(Vector2 direction)
        {
            if (!IsReady)
            {
                return;
            }

            Vector3 firePointPosition = firePoint.localPosition;
            firePointPosition.x = Mathf.Abs(firePointPosition.x)
                * Mathf.Sign(direction.x);
            firePoint.localPosition = firePointPosition;

            EnemyProjectile projectile = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity);

            projectile.Launch(direction, gameObject);
            nextShotTime = Time.time + shotCooldown;
        }
    }
}
