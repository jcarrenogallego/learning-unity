using Kogi.Scripts.Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kogi.Scripts.Player
{
    public sealed class KogiDaggerThrower : MonoBehaviour
    {
        [SerializeField]
        private Transform launchPoint;

        [SerializeField]
        private DaggerProjectile daggerPrefab;

        private void OnThrow(InputValue value)
        {
            if (!value.isPressed)
            {
                return;
            }

            ThrowDagger();
        }

        private void ThrowDagger()
        {
            Vector2 direction = launchPoint.localPosition.x < 0f
                ? Vector2.left
                : Vector2.right;

            DaggerProjectile dagger = Instantiate(
                daggerPrefab,
                launchPoint.position,
                Quaternion.identity);

            dagger.Launch(direction, gameObject);
        }
    }
}