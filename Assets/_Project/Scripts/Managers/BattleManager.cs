using System;
using _Project.Scripts.Runtime.Characters;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Managers
{
    public class BattleManager : MonoBehaviour
    {
        private AllyManager _allyManager;
        private EnemyManager _enemyManager;
        private ScreenManager _screenManager;
        
        private bool _initialized;

        [Inject]
        public void Construct(
            AllyManager allyManager,
            EnemyManager enemyManager,
            ScreenManager sceneManager)
        {
            _allyManager = allyManager;
            _enemyManager = enemyManager;
            _screenManager = sceneManager;
        }

        public void Initialize()
        {
            if (_initialized)
                return;
            Subscribe();
            _initialized = true;
        }

        public void Dispose()
        {
            if (!_initialized)
                return;
            Unsubscribe();
            _initialized = false;
        }

        private void Subscribe()
        {
            foreach (var unit in _allyManager.GetAllUnits()) 
                unit.OnUnitDied += OnUnitDied;
            
            foreach (var unit in _enemyManager.GetAllUnits()) 
                unit.OnUnitDied += OnUnitDied;
        }

        private void Unsubscribe()
        {
            foreach (var unit in _allyManager.GetAllUnits()) 
                unit.OnUnitDied -= OnUnitDied;
            
            foreach (var unit in _enemyManager.GetAllUnits()) 
                unit.OnUnitDied -= OnUnitDied;
        }

        private void OnUnitDied(UnitController unit)
        {
            HandleUnitDeath(unit);
        }

        private void HandleUnitDeath(UnitController unit)
        {
            if (unit.Team == TeamType.Enemy)
            {
                if (!_enemyManager.HasAliveEnemies())
                {
                    HandleVictory();
                }
            }

            if (unit.Team == TeamType.Ally)
            {
                if (!_allyManager.HasAliveAllies())
                {
                    HandleDefeat();
                }
            }
        }

        private void HandleVictory()
        {
            Debug.Log("🏆 Victory!");
            _ = _screenManager.GoNextScreen();
        }

        private void HandleDefeat()
        {
            Debug.Log("💀 Defeat!");
            _allyManager.ResetAllSession();
            // TODO: показати екран програшу, зупинити бій
        }
    }
}