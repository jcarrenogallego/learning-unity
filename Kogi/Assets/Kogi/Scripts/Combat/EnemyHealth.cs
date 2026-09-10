using UnityEngine;

namespace Kogi.Scripts.Combat
{
    public sealed class EnemyHealth : MonoBehaviour
    {
        [SerializeField, Min(1)]
        private int maximumHealth = 3;

        private int currentHealth;

        private void Awake()
        {
            currentHealth = maximumHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            Debug.Log($"Salud de {name}: {currentHealth}");

            if (currentHealth == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
