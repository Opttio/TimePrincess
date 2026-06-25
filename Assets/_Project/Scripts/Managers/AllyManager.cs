using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Core;
using _Project.Scripts.Runtime.Characters;
using _Project.Scripts.Runtime.Environment;
using _Project.Scripts.Runtime.Systems;
using _Project.Scripts.ScriptableObjects;
using UnityEngine;
using Zenject;
using Action = System.Action;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Managers
{
    public class AllyManager : MonoBehaviour
    {
        public event Action OnAlliesReassigned;
        
        [SerializeField] private List<UnitController> _allies = new List<UnitController>();
        [SerializeField] private Transform _allyInstancesBox;
        
        private AllySlot[] _activeSlots; // слоти на поточному Screen
        private List<UnitController> _alliesInstances = new List<UnitController>(); // інстанси у сцені
        
        private static readonly List<SlotType> FrontAllowed = new List<SlotType> { SlotType.Front, SlotType.Universal };
        private static readonly List<SlotType> BackAllowed = new List<SlotType> { SlotType.Back, SlotType.Universal };
        private static readonly List<SlotType> UniversalAllowed = new List<SlotType> { SlotType.Universal };
        
        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Start()
        {
            InstantiateAlliesInstances();
        }

        public void SetActiveScreenSlots(AllySlot[] slots)
        {
            _activeSlots = slots;
        }

        public List<Units> GetActiveUnitTypes()
        {
            var result = new List<Units>();
            foreach (var unit in GetActiveUnitControllers())
            {
                result.Add(unit.MyUnitData.unitType);
            }
            return result;
        }

        public UnitController GetUnitInSlot(int index)
        {
            if (_activeSlots == null || index < 0 || index >= _activeSlots.Length)
                return null;

            var slot = _activeSlots[index];

            return GetUnitFromSlot(slot);

        }

        public void ReassignAllies()
        {
            if (_activeSlots == null || _activeSlots.Length == 0 || _allies.Count == 0)
            {
                Debug.LogWarning("Немає слотів або юнітів!");
                return;
            }
            
            // 1. Очищаємо всі слоти
            foreach (var slot in _activeSlots)
                slot.ClearUnit();
            
            // 2. Сортуємо юнітів по Priority
            var sortedUnits = new List<UnitController>(_alliesInstances);
            sortedUnits.Sort((a, b) => GetPriority(b).CompareTo(GetPriority(a)));
            
            // 3. Робимо список вільних слотів
            var freeSlots = new List<AllySlot>(_activeSlots);
            
            // 4. Розставляємо юніти
            foreach (var unit in sortedUnits)
            {
                var allowedSlots = GetAllowedSlots(unit);

                AllySlot chosenSlot = null;

                // спочатку шукаємо **точний слот Front/Back**
                foreach (var slot in freeSlots)
                {
                    if (allowedSlots.Contains(slot.SlotType) && slot.SlotType != SlotType.Universal)
                    {
                        chosenSlot = slot;
                        break;
                    }
                }

                // якщо не знайшли → беремо Universal
                if (chosenSlot == null)
                {
                    foreach (var slot in freeSlots)
                    {
                        if (slot.SlotType == SlotType.Universal)
                        {
                            chosenSlot = slot;
                            break;
                        }
                    }
                }

                // якщо все ще null → беремо будь-який залишок
                if (chosenSlot == null && freeSlots.Count > 0)
                {
                    chosenSlot = freeSlots[0];
                }

                // 5. Ставимо юніта
                if (chosenSlot != null)
                {
                    unit.gameObject.SetActive(true);
                    chosenSlot.SetUnit(unit);
                    freeSlots.Remove(chosenSlot);
                }
                else
                {
                    Debug.LogWarning($"Не знайшли слот для {unit.name}");
                }
            }
            OnAlliesReassigned?.Invoke();
        }

        public UnitController GetTargetAlly(int incomingDamage)
        {
            var allUnits = GetActiveUnitControllers().Where(u => u!=null && u.CurrentHealth > 0).ToList();
            
            if (allUnits.Count == 0) return null;

            var chrona = allUnits.FirstOrDefault(u => u.MyUnitData.unitType == Units.PrincessChrona);
            if (chrona != null && allUnits.Count > 1)
            {
                int finalDamage = HealthSystem.ApplyArmor(incomingDamage, chrona.MyUnitData.armor);

                if (chrona.CurrentHealth <= finalDamage) 
                    allUnits.Remove(chrona);
            }
            
            var frontUnits = new List<UnitController>();
            var backUnits = new List<UnitController>();
            
            foreach (var unit in allUnits)
            {
                if (unit == null || unit.CurrentHealth <= 0)
                    continue;
                if (unit.CurrentSlot.SlotType == SlotType.Front)
                    frontUnits.Add(unit);
                if (unit.CurrentSlot.SlotType == SlotType.Back)
                    backUnits.Add(unit);
            }
            
            var pool = frontUnits.Count > 0 ? frontUnits : backUnits;
            if (pool.Count == 0) return null;
            return pool[Random.Range(0, pool.Count)];
        }

        public bool HasAliveAllies()
        {
            foreach (var unit in GetActiveUnitControllers())
            {
                if (unit != null && unit.CurrentHealth > 0)
                    return true;
            }

            return false;
        }
        
        public List<UnitController> GetAllUnits()
        {
            var result = new List<UnitController>();

            foreach (var unit in GetActiveUnitControllers())
            {
                if (unit != null)
                    result.Add(unit);
            }

            return result;
        }
        
        public UnitController GetUnitByType(Units unitType)
        {
            foreach (var unit in _alliesInstances)
                if (unit.MyUnitData.unitType == unitType)
                    return unit;
            return null;
        }

        public void ResetAllSession()
        {
            foreach (var unit in _alliesInstances)
                unit.ResetSession();
        }

        public void ApplyBonusUpgradeToAll(UpgradeData bonusUpgradeData)
        {
            foreach (var unit in _alliesInstances)
                unit.ApplyUpgrade(bonusUpgradeData);
        }

        public void AddAlly(UnitController prefab)
        {
            var instance = _container.InstantiatePrefabForComponent<UnitController>(prefab.gameObject, _allyInstancesBox);
            instance.gameObject.name = prefab.name;
            instance.gameObject.SetActive(false);
            
            _alliesInstances.Add(instance);
            
            ReassignAllies();
            OnAlliesReassigned?.Invoke();
        }

        public bool HasFreeSlot()
        {
            if (_activeSlots == null) return false;
            foreach (var slot in _activeSlots)
            {
                if (slot.OccupiedUnit == null)
                    return true;
            }
            return false;
        }

        private List<UnitController> GetActiveUnitControllers()
        {
            var activeUnits = new List<UnitController>();
            if (_activeSlots == null) return activeUnits;

            foreach (var slot in _activeSlots)
            {
                var unit = GetUnitFromSlot(slot);
                if (unit)
                    activeUnits.Add(unit);
            }

            return activeUnits;
        }

        private UnitController GetUnitFromSlot(AllySlot slot)
        {
            if (!slot) return null;
            return slot.OccupiedUnit;
        }

        private List<SlotType> GetAllowedSlots(UnitController unit)
        {
            switch (unit.MyUnitData.unitType)
            {
                case Units.BasicWarrior:
                case Units.KnightStelis:
                    return FrontAllowed;

                case Units.BasicArcher:
                case Units.PrincessChrona:
                    return BackAllowed;

                default:
                    return UniversalAllowed;
            }
        }

        private int GetPriority(UnitController unit)
        {
            return unit.MyUnitData.slotPriority;
        }

        private void InstantiateAlliesInstances()
        {
            // Очищаємо старі інстанси, якщо є
            foreach (var unit in _alliesInstances)
            {
                if (unit != null)
                    Destroy(unit.gameObject);
            }
            _alliesInstances.Clear();
            
            // Створюємо інстанси з префабів
            foreach (var prefab in _allies)
            {
                if (prefab == null) continue;

                var instance = _container.InstantiatePrefabForComponent<UnitController>(prefab.gameObject, _allyInstancesBox);
                instance.gameObject.name = prefab.name; // щоб було зрозуміле ім'я в ієрархії
                instance.gameObject.SetActive(false);
                _alliesInstances.Add(instance);
            }
        }
    }
}
