using System.Linq;
using UnityEngine;
using _Project.Scripts.ScriptableObjects;

namespace _Project.Scripts.Managers
{
    public class BonusManager
    {
        private readonly ChronaManager _chronaManager;
        private readonly BonusDatabase _database;

        public BonusManager(ChronaManager chronaManager, BonusDatabase database)
        {
            _chronaManager = chronaManager;
            _database = database;
        }

        public BonusData GetRandomBonus()
        {
            if (_chronaManager.IsFirstEncounter())
            {
                _chronaManager.RescueChrona();
                ReturnChrona();
            }
            var availableBonuses = _database.allBonuses.Where(b => b._type != BonusType.NewAlly || 
                                                                   b.allyPrefab.name != "Chrona").ToArray();
            
            if (availableBonuses.Length == 0)
            {
                Debug.LogWarning("База даних бонусів порожня або не містить доступних варіантів!");
                return null;
            }
            
            return availableBonuses[Random.Range(0, availableBonuses.Length)];
        }

        private BonusData ReturnChrona()
        {
            BonusData chronaBonus = _database.allBonuses.FirstOrDefault(b => b._type == BonusType.NewAlly &&
                                                                             b.allyPrefab != null &&
                                                                             b.allyPrefab.name != "Chrona");

            if (chronaBonus == null)
            {
                Debug.LogError("Хрону не знайдено в BonusDatabase. Перевір!");
            }
            
            Debug.Log("Отримав Хрону");
            return chronaBonus;
        }
    }
}