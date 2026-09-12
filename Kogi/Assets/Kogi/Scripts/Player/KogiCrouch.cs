using UnityEngine;
using UnityEngine.InputSystem;

namespace Kogi.Scripts.Player
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public sealed class KogiCrouch : MonoBehaviour
    {
        private const float CrouchHeightMultiplier = 0.5f;

        private CapsuleCollider2D bodyCollider;
        private Vector2 standingColliderSize;
        private Vector2 standingColliderOffset;

        public bool IsCrouching { get; private set; }

        private void Awake()
        {
            bodyCollider = GetComponent<CapsuleCollider2D>();
            standingColliderSize = bodyCollider.size;
            standingColliderOffset = bodyCollider.offset;
        }

        private void OnCrouch(InputValue value)
        {
            SetCrouching(value.isPressed);
        }

        private void SetCrouching(bool isCrouching)
        {
            IsCrouching = isCrouching;

            float multiplier = isCrouching ? CrouchHeightMultiplier : 1f;

            Vector2 colliderSize = standingColliderSize;
            colliderSize.y *= multiplier;

            float removedColliderHeight = standingColliderSize.y - colliderSize.y;
            Vector2 colliderOffset = standingColliderOffset;
            colliderOffset.y -= removedColliderHeight * 0.5f;

            bodyCollider.size = colliderSize;
            bodyCollider.offset = colliderOffset;

        }
    }
}
