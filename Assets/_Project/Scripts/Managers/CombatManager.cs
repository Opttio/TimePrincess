using System;
using _Project.Scripts.Core;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Managers
{
    public class CombatManager : MonoBehaviour
    {
        private AllyManager _allyManager;
        private EnemyManager _enemyManager;
        private GameEventBus _eventBus;

        [Inject]
        public void Construct(
            AllyManager allyManager,
            EnemyManager enemyManager,
            GameEventBus gameEventBus)
        {
            _allyManager = allyManager;
            _enemyManager = enemyManager;
            _eventBus = gameEventBus;
        }

        private void OnEnable()
        {
            _eventBus.OnPlayerAttack += HandlePlayerAttack;
            _eventBus.OnEnemyAttack += HandleEnemyAttack;
            _eventBus.OnPlayerHeal += HandlePlayerHeal;
        }

        private void OnDisable()
        {
            _eventBus.OnPlayerAttack -= HandlePlayerAttack;
            _eventBus.OnEnemyAttack -= HandleEnemyAttack;
            _eventBus.OnPlayerHeal -= HandlePlayerHeal;
        }

        private void HandlePlayerAttack(int damage)
        {
            var target = _enemyManager.GetTargetEnemy();
            
            if (target != null)
                target.TakeDamage(damage);
        }

        private void HandlePlayerHeal(int amount)
        {
            foreach (var unit in _allyManager.GetAllUnits())
            {
                unit.Heal(amount);
            }
        }

        private void HandleEnemyAttack(int damage)
        {
            var target = _allyManager.GetTargetAlly(damage);
            
            if (target != null)
                target.TakeDamage(damage);
        }
    }
}