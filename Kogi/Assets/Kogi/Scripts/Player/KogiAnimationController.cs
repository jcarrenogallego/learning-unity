using UnityEngine;

namespace Kogi.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(KogiMovement))]
    [RequireComponent(typeof(KogiCrouch))]
    public sealed class KogiAnimationController : MonoBehaviour
    {
        private static readonly int SpeedParameter = Animator.StringToHash("Speed");
        private static readonly int VerticalSpeedParameter = Animator.StringToHash("VerticalSpeed");
        private static readonly int IsGroundedParameter = Animator.StringToHash("IsGrounded");
        private static readonly int IsCrouchingParameter = Animator.StringToHash("IsCrouching");

        private Animator animator;
        private KogiMovement movement;
        private KogiCrouch crouch;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            movement = GetComponent<KogiMovement>();
            crouch = GetComponent<KogiCrouch>();
        }

        private void Update()
        {
            animator.SetFloat(SpeedParameter, movement.HorizontalSpeed);
            animator.SetFloat(VerticalSpeedParameter, movement.VerticalSpeed);
            animator.SetBool(IsGroundedParameter, movement.IsGrounded);
            animator.SetBool(IsCrouchingParameter, crouch.IsCrouching);
        }
    }
}
