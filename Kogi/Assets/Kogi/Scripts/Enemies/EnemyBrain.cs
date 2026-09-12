using Kogi.Scripts.Combat;
using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    [RequireComponent(typeof(EnemyVision))]
    [RequireComponent(typeof(EnemyPatrol))]
    [RequireComponent(typeof(EnemyShooter))]
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(EnemyVisualFeedback))]
    public sealed class EnemyBrain : MonoBehaviour
    {
        private enum EnemyState
        {
            Patrol,
            PrepareAttack,
            Attack,
            Hurt,
            Dead
        }

        private EnemyVision vision;
        private EnemyPatrol patrol;
        private EnemyShooter shooter;
        private EnemyHealth health;
        private Rigidbody2D body;
        private Collider2D bodyCollider;
        private EnemyVisualFeedback visualFeedback;

        [SerializeField, Min(0f)]
        private float attackPreparationDuration = 0.6f;

        [SerializeField, Min(0f)]
        private float hurtDuration = 0.25f;

        [SerializeField, Min(0f)]
        private float deathDelay = 0.5f;

        private float remainingHurtTime;
        private float remainingAttackPreparation;
        private EnemyState currentState;
        private bool hasCurrentState;

        private void Awake()
        {
            vision = GetComponent<EnemyVision>();
            patrol = GetComponent<EnemyPatrol>();
            shooter = GetComponent<EnemyShooter>();
            health = GetComponent<EnemyHealth>();
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<Collider2D>();
            visualFeedback = GetComponent<EnemyVisualFeedback>();
        }

        private void OnEnable()
        {
            health.Damaged += HandleDamaged;
            health.Died += HandleDied;
        }

        private void OnDisable()
        {
            health.Damaged -= HandleDamaged;
            health.Died -= HandleDied;
        }

        private void Update()
        {
            if (currentState == EnemyState.Dead)
            {
                return;
            }

            if (remainingHurtTime > 0f)
            {
                remainingHurtTime -= Time.deltaTime;
                ChangeState(EnemyState.Hurt);
                return;
            }

            if (!vision.CanSeeTarget)
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (currentState == EnemyState.PrepareAttack)
            {
                remainingAttackPreparation -= Time.deltaTime;

                if (remainingAttackPreparation <= 0f)
                {
                    shooter.Shoot(vision.DirectionToTarget);
                    ChangeState(EnemyState.Attack);
                }

                return;
            }

            if (shooter.IsReady)
            {
                remainingAttackPreparation = attackPreparationDuration;
                ChangeState(EnemyState.PrepareAttack);
                return;
            }

            ChangeState(EnemyState.Attack);
        }

        private void HandleDamaged()
        {
            remainingAttackPreparation = 0f;
            remainingHurtTime = hurtDuration;
            ChangeState(EnemyState.Hurt);
        }

        private void HandleDied()
        {
            remainingAttackPreparation = 0f;
            ChangeState(EnemyState.Dead);
            body.linearVelocity = Vector2.zero;
            body.simulated = false;
            bodyCollider.enabled = false;
            Destroy(gameObject, deathDelay);
        }

        private void ChangeState(EnemyState nextState)
        {
            if (hasCurrentState && currentState == nextState)
            {
                return;
            }

            currentState = nextState;
            hasCurrentState = true;

            patrol.enabled = currentState == EnemyState.Patrol;

            switch (currentState)
            {
                case EnemyState.PrepareAttack:
                    visualFeedback.ShowAttackPreparation();
                    break;
                case EnemyState.Hurt:
                    visualFeedback.ShowHurt();
                    break;
                case EnemyState.Dead:
                    visualFeedback.ShowDead();
                    break;
                default:
                    visualFeedback.ShowNormal();
                    break;
            }

            Debug.Log($"{name} cambia a {currentState}");
        }
    }
}
