using UnityEngine;
using UnityEngine.InputSystem;

namespace Kogi.Scripts.Player
{
    public sealed class KogiFacing : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer characterRenderer;

        [SerializeField]
        private Transform attackPoint;

        [SerializeField, Min(0f)]
        private float attackDistance = 0.75f;

        private void OnMove(InputValue value)
        {
            float horizontalDirection = value.Get<Vector2>().x;

            if (Mathf.Approximately(horizontalDirection, 0f))
            {
                return;
            }

            bool isFacingLeft = horizontalDirection < 0f;
            characterRenderer.flipX = isFacingLeft;

            Vector3 attackPosition = attackPoint.localPosition;
            attackPosition.x = isFacingLeft ? -attackDistance : attackDistance;
            attackPoint.localPosition = attackPosition;
        }
    }
}
