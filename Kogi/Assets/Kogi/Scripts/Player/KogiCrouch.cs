using UnityEngine;
using UnityEngine.InputSystem;

namespace Kogi.Scripts.Player
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public sealed class KogiCrouch : MonoBehaviour
    {
        private const float CrouchHeightMultiplier = 0.5f;

        [SerializeField]
        private Transform characterVisual;

        private CapsuleCollider2D bodyCollider;
        private Vector2 standingColliderSize;
        private Vector2 standingColliderOffset;
        private Vector3 standingVisualScale;
        private Vector3 standingVisualPosition;

        private void Awake()
        {
            bodyCollider = GetComponent<CapsuleCollider2D>();
            standingColliderSize = bodyCollider.size;
            standingColliderOffset = bodyCollider.offset;
            standingVisualScale = characterVisual.localScale;
            standingVisualPosition = characterVisual.localPosition;
        }

        private void OnCrouch(InputValue value)
        {
            SetCrouching(value.isPressed);
        }

        private void SetCrouching(bool isCrouching)
        {
            float multiplier = isCrouching ? CrouchHeightMultiplier : 1f;

            Vector2 colliderSize = standingColliderSize;
            colliderSize.y *= multiplier;

            float removedColliderHeight = standingColliderSize.y - colliderSize.y;
            Vector2 colliderOffset = standingColliderOffset;
            colliderOffset.y -= removedColliderHeight * 0.5f;

            bodyCollider.size = colliderSize;
            bodyCollider.offset = colliderOffset;

            Vector3 visualScale = standingVisualScale;
            visualScale.y *= multiplier;

            float removedVisualHeight = standingVisualScale.y - visualScale.y;
            Vector3 visualPosition = standingVisualPosition;
            visualPosition.y -= removedVisualHeight * 0.5f;

            characterVisual.localScale = visualScale;
            characterVisual.localPosition = visualPosition;
        }
    }
}
