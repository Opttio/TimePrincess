using System;
using _Project.Scripts.Runtime.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Enemy
{
    public class EnemyHealthBarView : MonoBehaviour
    {
        [SerializeField] private Image _fillHealthBar;
        
        private UnitController _unit;

        private void Start()
        {
            _unit = GetComponentInParent<UnitController>();
            _unit.OnHealthChanged += UpdateBar;
            UpdateBar(_unit.CurrentHealth, _unit.MaxHealth);
        }

        private void OnDestroy()
        {
            if (_unit != null)
                _unit.OnHealthChanged -= UpdateBar;
        }

        private void UpdateBar(int currentHealth, int maxHealth)
        {
            _fillHealthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}