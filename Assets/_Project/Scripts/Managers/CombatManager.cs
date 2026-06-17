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
        }

        private void OnDisable()
        {
            _eventBus.OnPlayerAttack -= HandlePlayerAttack;
            _eventBus.OnEnemyAttack -= HandleEnemyAttack;
        }

        private void HandlePlayerAttack(int damage)
        {
            var target = _enemyManager.GetTargetEnemy();
            
            if (target != null)
                target.TakeDamage(damage);
        }

        private void HandleEnemyAttack(int damage)
        {
            var target = _allyManager.GetTargetAlly();
            
            if (target != null)
                target.TakeDamage(damage);
        }
    }
}