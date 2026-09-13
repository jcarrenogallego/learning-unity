using Kogi.Scripts.Combat;
using UnityEngine;
using System.Collections;
using Kogi.Scripts.UI;

namespace Kogi.Scripts.Enemies
{
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(EnemyShooter))]
    [RequireComponent(typeof(EnemyVision))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class DesertWarden : MonoBehaviour
    {
        [SerializeField, Min(2)]
        private int maximumHealth = 12;

        [SerializeField, Range(0.1f, 0.9f)]
        private float secondPhaseThreshold = 0.5f;

        [SerializeField, Min(0f)]
        private float deathDelay = 0.5f;

        private EnemyHealth health;
        private EnemyShooter shooter;
        private EnemyVision vision;
        private SpriteRenderer visual;
        private bool secondPhase;
        private bool defeated;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
            shooter = GetComponent<EnemyShooter>();
            vision = GetComponent<EnemyVision>();
            visual = GetComponent<SpriteRenderer>();
            health.Configure(maximumHealth);
            shooter.Configure(1.6f);
            vision.Configure(10f);
        }

        private void OnEnable()
        {
            health.Died += HandleDefeated;
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.Died -= HandleDefeated;
            }
        }

        private void Update()
        {
            if (defeated)
            {
                return;
            }

            float healthRatio = (float)health.CurrentHealth / health.MaximumHealth;

            if (!secondPhase && healthRatio <= secondPhaseThreshold)
            {
                EnterSecondPhase();
            }

            if (vision.CanSeeTarget && shooter.IsReady)
            {
                shooter.Shoot(vision.DirectionToTarget);
            }
        }

        private void EnterSecondPhase()
        {
            secondPhase = true;
            shooter.Configure(0.75f);
            visual.color = new Color(0.9f, 0.3f, 0.75f);
            transform.localScale *= 1.08f;
            Debug.Log("Guardián del Desierto entra en su segunda fase");
        }

        private void HandleDefeated()
        {
            if (defeated)
            {
                return;
            }

            defeated = true;
            shooter.enabled = false;
            vision.enabled = false;

            // The boss replaces EnemyBrain, so it also owns death cleanup.
            foreach (EnemyContactDamage contactDamage in GetComponentsInChildren<EnemyContactDamage>(true))
            {
                contactDamage.enabled = false;
            }

            foreach (Collider2D bodyCollider in GetComponentsInChildren<Collider2D>(true))
            {
                bodyCollider.enabled = false;
            }

            foreach (Rigidbody2D body in GetComponentsInChildren<Rigidbody2D>(true))
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
                body.simulated = false;
            }

            visual.color = Color.gray;
            StartCoroutine(FinishDefeat());
            Debug.Log("Guardián del Desierto derrotado");
        }

        private IEnumerator FinishDefeat()
        {
            yield return new WaitForSeconds(deathDelay);
            if (GameFlowController.Instance != null) GameFlowController.Instance.ShowVictory();
            Destroy(gameObject);
        }

        private void OnGUI()
        {
            if (defeated || health == null)
            {
                return;
            }

            float width = Mathf.Min(520f, Screen.width - 80f);
            Rect background = new Rect((Screen.width - width) * 0.5f, 28f, width, 24f);
            float ratio = (float)health.CurrentHealth / health.MaximumHealth;
            GUI.Box(background, string.Empty);
            Color previous = GUI.color;
            GUI.color = secondPhase ? new Color(0.9f, 0.25f, 0.7f) : new Color(0.65f, 0.18f, 0.12f);
            GUI.DrawTexture(new Rect(background.x + 3f, background.y + 3f, (background.width - 6f) * ratio, background.height - 6f), Texture2D.whiteTexture);
            GUI.color = previous;
            GUI.Label(new Rect(background.x, background.y + 26f, width, 25f), "Guardián del Desierto");
        }
    }
}
