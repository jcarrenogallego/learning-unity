using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    [RequireComponent(typeof(EnemyVision))]
    [RequireComponent(typeof(EnemyPatrol))]
    [RequireComponent(typeof(EnemyShooter))]
    public sealed class EnemyBrain : MonoBehaviour
    {
        private enum EnemyState
        {
            Patrol,
            Attack
        }

        private EnemyVision vision;
        private EnemyPatrol patrol;
        private EnemyShooter shooter;
        private EnemyState currentState;
        private bool hasCurrentState;

        private void Awake()
        {
            vision = GetComponent<EnemyVision>();
            patrol = GetComponent<EnemyPatrol>();
            shooter = GetComponent<EnemyShooter>();
        }

        private void Update()
        {
            EnemyState nextState = vision.CanSeeTarget
                ? EnemyState.Attack
                : EnemyState.Patrol;

            ChangeState(nextState);
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
