using UnityEngine;

namespace Kogi.Scripts.Player.Visual
{
    public sealed class KogiRigAnimator : MonoBehaviour
    {
        [SerializeField] private KogiMovement movement;
        [SerializeField] private KogiCrouch crouch;
        [SerializeField] private KogiAttack attack;
        [SerializeField] private Transform torso;
        [SerializeField] private Transform head;
        [SerializeField] private Transform armBack;
        [SerializeField] private Transform armFront;
        [SerializeField] private Transform legBack;
        [SerializeField] private Transform legFront;
        [SerializeField] private Transform scarf;

        private const float AttackDuration = 0.22f;
        private float attackTimeRemaining;
        private Vector3 basePosition;
        private Vector3 baseScale;

        private void Awake()
        {
            basePosition = transform.localPosition;
            baseScale = transform.localScale;
        }

        private void OnEnable()
        {
            attack.AttackPerformed += PlayAttack;
        }

        private void OnDisable()
        {
            attack.AttackPerformed -= PlayAttack;
        }

        private void LateUpdate()
        {
            attackTimeRemaining = Mathf.Max(0f, attackTimeRemaining - Time.deltaTime);
            ResetPose();

            if (attackTimeRemaining > 0f)
            {
                ApplyAttackPose();
            }
            else if (crouch.IsCrouching)
            {
                ApplyCrouchPose();
            }
            else if (!movement.IsGrounded)
            {
                ApplyAirPose(movement.VerticalSpeed);
            }
            else if (movement.HorizontalSpeed > 0.1f)
            {
                ApplyRunPose();
            }
            else
            {
                ApplyIdlePose();
            }
        }

        private void ResetPose()
        {
            float facingSign = Mathf.Sign(transform.localScale.x);
            transform.localPosition = basePosition;
            transform.localScale = new Vector3(Mathf.Abs(baseScale.x) * facingSign, baseScale.y, baseScale.z);
            torso.localRotation = Quaternion.identity;
            head.localRotation = Quaternion.identity;
            armBack.localRotation = Quaternion.identity;
            armFront.localRotation = Quaternion.identity;
            legBack.localRotation = Quaternion.identity;
            legFront.localRotation = Quaternion.identity;
            scarf.localRotation = Quaternion.identity;
        }

        private void ApplyIdlePose()
        {
            float wave = Mathf.Sin(Time.time * 2.2f);
            torso.localRotation = Quaternion.Euler(0f, 0f, wave * 1.2f);
            head.localRotation = Quaternion.Euler(0f, 0f, -wave * 1.8f);
            armFront.localRotation = Quaternion.Euler(0f, 0f, wave * 2f);
            scarf.localRotation = Quaternion.Euler(0f, 0f, -4f + wave * 3f);
            transform.localScale = new Vector3(transform.localScale.x, baseScale.y * (1f + wave * 0.012f), baseScale.z);
        }

        private void ApplyRunPose()
        {
            float cycle = Mathf.Sin(Time.time * 10f);
            transform.localPosition = basePosition + Vector3.up * Mathf.Abs(cycle) * 0.035f;
            torso.localRotation = Quaternion.Euler(0f, 0f, -5f);
            head.localRotation = Quaternion.Euler(0f, 0f, 4f);
            armBack.localRotation = Quaternion.Euler(0f, 0f, cycle * 24f);
            armFront.localRotation = Quaternion.Euler(0f, 0f, -cycle * 24f);
            legBack.localRotation = Quaternion.Euler(0f, 0f, -cycle * 28f);
            legFront.localRotation = Quaternion.Euler(0f, 0f, cycle * 28f);
            scarf.localRotation = Quaternion.Euler(0f, 0f, -14f + cycle * 5f);
        }

        private void ApplyAirPose(float verticalSpeed)
        {
            float direction = verticalSpeed >= 0f ? 1f : -1f;
            torso.localRotation = Quaternion.Euler(0f, 0f, -4f * direction);
            armBack.localRotation = Quaternion.Euler(0f, 0f, -28f);
            armFront.localRotation = Quaternion.Euler(0f, 0f, 24f);
            legBack.localRotation = Quaternion.Euler(0f, 0f, 18f * direction);
            legFront.localRotation = Quaternion.Euler(0f, 0f, -20f * direction);
            scarf.localRotation = Quaternion.Euler(0f, 0f, -20f);
            float stretch = verticalSpeed >= 0f ? 1.05f : 0.96f;
            transform.localScale = new Vector3(transform.localScale.x / stretch, baseScale.y * stretch, baseScale.z);
        }

        private void ApplyCrouchPose()
        {
            transform.localPosition = basePosition + Vector3.down * 0.22f;
            transform.localScale = new Vector3(transform.localScale.x * 1.08f, baseScale.y * 0.72f, baseScale.z);
            torso.localRotation = Quaternion.Euler(0f, 0f, -7f);
            armFront.localRotation = Quaternion.Euler(0f, 0f, -18f);
            legBack.localRotation = Quaternion.Euler(0f, 0f, 25f);
            legFront.localRotation = Quaternion.Euler(0f, 0f, -25f);
        }

        private void ApplyAttackPose()
        {
            float progress = 1f - (attackTimeRemaining / AttackDuration);
            float arc = Mathf.Sin(progress * Mathf.PI);
            torso.localRotation = Quaternion.Euler(0f, 0f, -8f * arc);
            head.localRotation = Quaternion.Euler(0f, 0f, 5f * arc);
            armFront.localRotation = Quaternion.Euler(0f, 0f, -95f * arc);
            armBack.localRotation = Quaternion.Euler(0f, 0f, 20f * arc);
            scarf.localRotation = Quaternion.Euler(0f, 0f, -18f * arc);
        }

        private void PlayAttack()
        {
            attackTimeRemaining = AttackDuration;
        }
    }
}
