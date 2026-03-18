using System.Collections.Generic;
using _Project.Scripts.Runtime.Characters;
using _Project.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class AllyManager : MonoBehaviour
    {
        private Transform[] _activeSlots; // масив точок на поточному Screen

        /// <summary>
        /// Викликається ScreenManager при перемиканні Screen.
        /// Передає масив Transform слотів, де можуть стояти союзники.
        /// </summary>
        public void SetActiveScreenSlots(Transform[] slots)
        {
            _activeSlots = slots;
        }

        /// <summary>
        /// Повертає список типів юнітів, які зараз стоять у слотах
        /// </summary>
        public List<Units> GetActiveUnits()
        {
            var activeUnits = new List<Units>();
            if (_activeSlots == null) return activeUnits;

            foreach (var slot in _activeSlots)
            {
                if (slot.childCount > 0)
                {
                    var unitController = slot.GetChild(0).GetComponent<UnitController>();
                    if (unitController)
                        activeUnits.Add(unitController.MyUnitData.unitType);
                }
            }

            return activeUnits;
        }

        /// <summary>
        /// Повертає UnitData об'єкта у слоті за індексом
        /// </summary>
        public UnitsData GetUnitDataInSlot(int index)
        {
            if (_activeSlots == null || index < 0 || index >= _activeSlots.Length) return null;

            var slot = _activeSlots[index];
            if (slot.childCount > 0)
            {
                var unitController = slot.GetChild(0).GetComponent<UnitController>();
                return unitController?.MyUnitData;
            }

            return null;
        }

        /// <summary>
        /// Перевіряє, чи є вільний слот для нового союзника
        /// </summary>
        public bool HasFreeSlot()
        {
            if (_activeSlots == null) return false;

            foreach (var slot in _activeSlots)
            {
                if (slot.childCount == 0)
                    return true;
            }

            return false;
        }
    }
}
