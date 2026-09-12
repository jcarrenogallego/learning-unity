using Kogi.Scripts.Projectiles;
using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    [RequireComponent(typeof(EnemyVision))]
    public sealed class EnemyShooter : MonoBehaviour
    {
        [SerializeField]
        private Transform firePoint;

        [SerializeField]
        private EnemyProjectile projectilePrefab;

        [SerializeField, Min(0.1f)]
        private float shotCooldown = 2f;

        private float remainingCooldown;
        private EnemyVision vision;

        private void Awake()
        {
            vision = GetComponent<EnemyVision>();
        }

        private void Update()
        {
            remainingCooldown -= Time.deltaTime;

            if (!vision.CanSeeTarget || remainingCooldown > 0f)
            {
                return;
            }

            Shoot(vision.DirectionToTarget);
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
