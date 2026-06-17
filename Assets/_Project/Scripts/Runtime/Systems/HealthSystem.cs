using _Project.Scripts.Runtime.RuntimeData;
using UnityEngine;
using System;

namespace _Project.Scripts.Runtime.Systems
{
    public class HealthSystem
    {
        public event Action<int, int> OnHealthChanged;
        public event Action OnDeath;
        
        private HealthRuntimeData _healthsData;

        public HealthSystem(HealthRuntimeData healthsData)
        {
            _healthsData = healthsData;
        }
        
        public void TakeDamage(int damage)
        {
            if (_healthsData.currentHealth <= 0)
                return;

            int finalDamage = ApplyArmor(damage, _healthsData.armor);
            _healthsData.currentHealth -= finalDamage;
            _healthsData.currentHealth = Mathf.Max(_healthsData.currentHealth, 0);
            
            OnHealthChanged?.Invoke(_healthsData.currentHealth, _healthsData.maxHealth);

            if (_healthsData.currentHealth <= 0)
            {
                Death();
            }
            
        }

        public void Heal(int heal)
        {
            if (_healthsData.currentHealth >= _healthsData.maxHealth)
                return;
            _healthsData.currentHealth += heal;
            _healthsData.currentHealth = Mathf.Min(_healthsData.currentHealth, _healthsData.maxHealth);
        }
        
        private int ApplyArmor(int damage, float armor)
        {
            int finalDmg = Mathf.FloorToInt(damage * (100f / (100f + armor)));
            return Mathf.Max(finalDmg, 1); // мінімум 1 dmg
        }

        private void Death()
        {
            OnDeath?.Invoke();
        }
    }
}