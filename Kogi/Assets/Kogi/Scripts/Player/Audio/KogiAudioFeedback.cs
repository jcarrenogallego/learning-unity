using UnityEngine;

namespace Kogi.Scripts.Player.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class KogiAudioFeedback : MonoBehaviour
    {
        [SerializeField] private KogiMovement movement;
        [SerializeField] private KogiAttack attack;
        [SerializeField] private KogiDamageReceiver damageReceiver;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip attackClip;
        [SerializeField] private AudioClip hurtClip;

        private AudioSource source;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            movement.Jumped += PlayJump;
            attack.AttackPerformed += PlayAttack;
            damageReceiver.HitReceived += PlayHurt;
        }

        private void OnDisable()
        {
            movement.Jumped -= PlayJump;
            attack.AttackPerformed -= PlayAttack;
            damageReceiver.HitReceived -= PlayHurt;
        }

        private void PlayJump() => source.PlayOneShot(jumpClip, 0.45f);
        private void PlayAttack() => source.PlayOneShot(attackClip, 0.55f);
        private void PlayHurt() => source.PlayOneShot(hurtClip, 0.65f);
    }
}
