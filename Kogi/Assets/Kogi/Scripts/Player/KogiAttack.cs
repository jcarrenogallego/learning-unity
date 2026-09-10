using Kogi.Scripts.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kogi.Scripts.Player
{
    public sealed class KogiAttack : MonoBehaviour
    {
        [SerializeField]
        private Transform attackPoint;

        [SerializeField]
        private LayerMask enemyLayer;

        [SerializeField, Min(0f)]
        private float attackRadius = 0.75f;

        [SerializeField, Min(1)]
        private int damage = 1;

        private void OnAttack(InputValue value)
        {
            if (!value.isPressed)
            {
                return;
            }

            Attack();
        }

        private void Attack()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius,
                enemyLayer);

            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out EnemyHealth enemy))
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint is null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
