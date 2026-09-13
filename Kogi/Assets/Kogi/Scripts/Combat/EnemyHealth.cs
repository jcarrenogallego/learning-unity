using System;
using UnityEngine;

namespace Kogi.Scripts.Combat
{
    public sealed class EnemyHealth : MonoBehaviour
    {
        public event Action Damaged;
        public event Action Died;

        public int CurrentHealth { get; private set; }
        public int MaximumHealth => maximumHealth;

        [SerializeField, Min(1)]
        private int maximumHealth = 3;

        private void Awake()
        {
            CurrentHealth = maximumHealth;
        }

        public void Configure(int health)
        {
            maximumHealth = Mathf.Max(1, health);
            CurrentHealth = maximumHealth;
        }

        public void TakeDamage(int damage)
        {
            if (CurrentHealth == 0)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
            Debug.Log($"Salud de {name}: {CurrentHealth}");

            if (CurrentHealth == 0)
            {
                Died?.Invoke();
                return;
            }

            Damaged?.Invoke();
        }
    }
}
