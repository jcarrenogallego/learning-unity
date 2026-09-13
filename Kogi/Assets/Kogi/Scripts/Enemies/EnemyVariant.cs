using Kogi.Scripts.Combat;
using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(EnemyPatrol))]
    [RequireComponent(typeof(EnemyShooter))]
    [RequireComponent(typeof(EnemyVision))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class EnemyVariant : MonoBehaviour
    {
        public enum Variant
        {
            Scout,
            Guardian,
            Elite
        }

        [SerializeField]
        private Variant variant = Variant.Guardian;

        public Variant CurrentVariant => variant;

        private void Awake()
        {
            Apply();
        }

        public void Configure(Variant selectedVariant)
        {
            variant = selectedVariant;
            Apply();
        }

        private void Apply()
        {
            EnemyHealth health = GetComponent<EnemyHealth>();
            EnemyPatrol patrol = GetComponent<EnemyPatrol>();
            EnemyShooter shooter = GetComponent<EnemyShooter>();
            EnemyVision vision = GetComponent<EnemyVision>();
            SpriteRenderer visual = GetComponent<SpriteRenderer>();

            switch (variant)
            {
                case Variant.Scout:
                    health.Configure(2);
                    patrol.Configure(3.4f, 2.5f);
                    shooter.Configure(2.4f);
                    vision.Configure(7f);
                    visual.color = new Color(0.95f, 0.65f, 0.3f);
                    break;
                case Variant.Elite:
                    health.Configure(6);
                    patrol.Configure(1.4f, 1.5f);
                    shooter.Configure(1.25f);
                    vision.Configure(8f);
                    visual.color = new Color(0.55f, 0.25f, 0.8f);
                    break;
                default:
                    health.Configure(3);
                    patrol.Configure(2f, 2f);
                    shooter.Configure(2f);
                    vision.Configure(6f);
                    visual.color = new Color(0.72f, 0.22f, 0.2f);
                    break;
            }
        }
    }
}
