using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Runtime.Characters;
using _Project.Scripts.Runtime.Environment;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class EnemyManager : MonoBehaviour
    {
        private EnemySlot[] _currentSlots;

        public void SetActiveSlots(EnemySlot[] slots)
        {
            _currentSlots = slots;
        }

        public UnitController GetTargetEnemy()
        {
            if (_currentSlots == null) return null;
            
            var frontUnits = new List<UnitController>();
            var backUnits = new List<UnitController>();

            foreach (var slot in _currentSlots)
            {
                var unit = slot.OccupiedUnit;
                if (unit == null || unit.CurrentHealth <= 0)
                    continue;
                if (slot.SlotType == SlotType.Front)
                    frontUnits.Add(unit);
                else if (slot.SlotType == SlotType.Back)
                    backUnits.Add(unit);
            }
            var pool = frontUnits.Count > 0 ? frontUnits : backUnits;
            if (pool.Count == 0) return null;
            return pool[Random.Range(0, pool.Count)];
        }

        public bool HasAliveEnemies()
        {
            foreach (var slot in _currentSlots)
            {
                var unit = slot.OccupiedUnit;

                if (unit != null && unit.CurrentHealth > 0)
                    return true;
            }

            return false;
        }
        
        public List<UnitController> GetAllUnits()
        {
            var result = new List<UnitController>();

            foreach (var slot in _currentSlots)
            {
                if (slot.OccupiedUnit != null)
                    result.Add(slot.OccupiedUnit);
            }

            return result;
        }
    }
}