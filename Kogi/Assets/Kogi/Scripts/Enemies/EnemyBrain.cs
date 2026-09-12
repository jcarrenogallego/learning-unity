using Kogi.Scripts.Combat;
using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    [RequireComponent(typeof(EnemyVision))]
    [RequireComponent(typeof(EnemyPatrol))]
    [RequireComponent(typeof(EnemyShooter))]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyBrain : MonoBehaviour
    {
        private enum EnemyState
        {
            Patrol,
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

        [SerializeField, Min(0f)]
        private float hurtDuration = 0.25f;

        [SerializeField, Min(0f)]
        private float deathDelay = 0.5f;

        private float remainingHurtTime;
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

            EnemyState nextState = vision.CanSeeTarget
                ? EnemyState.Attack
                : EnemyState.Patrol;

            ChangeState(nextState);
        }

        private void HandleDamaged()
        {
            remainingHurtTime = hurtDuration;
            ChangeState(EnemyState.Hurt);
        }

        private void HandleDied()
        {
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
            shooter.enabled = currentState == EnemyState.Attack;

            Debug.Log($"{name} cambia a {currentState}");
        }
    }
}
