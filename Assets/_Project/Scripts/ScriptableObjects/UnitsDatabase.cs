using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "UnitsDatabase", menuName = "Units/UnitsDatabase", order = 1)]
    public class UnitsDatabase : ScriptableObject
    {
        [SerializeField] private List<UnitsData> _allUnits;

        private Dictionary<Units, UnitsData> _lookup;

        private void OnEnable()
        {
            // будуємо словник для швидкого пошуку
            _lookup = _allUnits.ToDictionary(u => u.unitType, u => u);
        }

        public UnitsData GetData(Units unitType)
        {
            if (_lookup == null || _lookup.Count == 0)
                OnEnable(); // страховка на випадок, якщо SO ще не ініціалізований

            if (_lookup.TryGetValue(unitType, out var data))
                return data;

            Debug.LogWarning($"[UnitsDatabase] Не знайдено UnitsData для типу: {unitType}");
            return null;
        }

        public Sprite GetIcon(Units unitType)
        {
            var data = GetData(unitType);
            return data != null ? data.unitIcon : null;
        }
    }
}