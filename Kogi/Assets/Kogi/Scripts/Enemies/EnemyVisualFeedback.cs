using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class EnemyVisualFeedback : MonoBehaviour
    {
        [SerializeField]
        private Color attackPreparationColor = Color.yellow;

        [SerializeField]
        private Color hurtColor = Color.white;

        [SerializeField]
        private Color deadColor = Color.gray;

        private SpriteRenderer characterRenderer;
        private Color normalColor;

        private void Awake()
        {
            characterRenderer = GetComponent<SpriteRenderer>();
            normalColor = characterRenderer.color;
        }

        public void ShowNormal()
        {
            characterRenderer.color = normalColor;
        }

        public void ShowAttackPreparation()
        {
            characterRenderer.color = attackPreparationColor;
        }

        public void ShowHurt()
        {
            characterRenderer.color = hurtColor;
        }

        public void ShowDead()
        {
            characterRenderer.color = deadColor;
        }
    }
}
