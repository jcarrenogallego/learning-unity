using UnityEngine;
using UnityEngine.InputSystem;

namespace Kogi.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class KogiMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float speed = 5f;

        [SerializeField, Min(0f)]
        private float jumpForce = 8f;

        [SerializeField]
        private Transform groundCheck;

        [SerializeField]
        private LayerMask groundLayer;

        [SerializeField, Min(0f)]
        private float groundCheckRadius = 0.15f;

        private Rigidbody2D body;
        private Vector2 movementInput;
        private bool jumpRequested;

        public float HorizontalSpeed => Mathf.Abs(body.linearVelocity.x);

        public float VerticalSpeed => body.linearVelocity.y;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void OnMove(InputValue value)
        {
            movementInput = value.Get<Vector2>();
        }

        private void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                jumpRequested = true;
            }
        }

        private void FixedUpdate()
        {
            IsGrounded = CheckIsGrounded();

            Vector2 velocity = body.linearVelocity;
            velocity.x = movementInput.x * speed;

            if (jumpRequested && IsGrounded)
            {
                velocity.y = jumpForce;
            }

            body.linearVelocity = velocity;
            jumpRequested = false;
        }

        private bool CheckIsGrounded()
        {
            return Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer) is not null;
        }
    }
}
