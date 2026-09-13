using Kogi.Scripts.Player;
using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    public sealed class EnemyVision : MonoBehaviour
    {
        [SerializeField]
        private Transform visionPoint;

        [SerializeField, Min(0f)]
        private float detectionRange = 6f;

        [SerializeField]
        private LayerMask visibleLayers;

        private Transform target;

        public bool CanSeeTarget { get; private set; }

        public Vector2 DirectionToTarget => target is null
            ? Vector2.zero
            : target.position - visionPoint.position;

        public void Configure(float range)
        {
            detectionRange = Mathf.Max(0f, range);
        }

        private void Start()
        {
            KogiDamageReceiver receiver = FindAnyObjectByType<KogiDamageReceiver>();

            if (receiver is not null)
            {
                target = receiver.transform;
            }
        }

        private void Update()
        {
            CanSeeTarget = CalculateLineOfSight();

            if (target is null)
            {
                return;
            }

            Debug.DrawRay(
                visionPoint.position,
                DirectionToTarget.normalized * detectionRange,
                CanSeeTarget ? Color.green : Color.red);
        }

        private bool CalculateLineOfSight()
        {
            if (target is null || DirectionToTarget.magnitude > detectionRange)
            {
                return false;
            }

            RaycastHit2D hit = Physics2D.Raycast(
                visionPoint.position,
                DirectionToTarget.normalized,
                detectionRange,
                visibleLayers);

            return hit.collider is not null
                && hit.collider.TryGetComponent(out KogiDamageReceiver _);
        }
    }
}
