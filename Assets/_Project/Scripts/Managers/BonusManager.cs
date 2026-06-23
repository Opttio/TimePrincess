using System.Linq;
using UnityEngine;
using _Project.Scripts.ScriptableObjects;
using _Project.Scripts.UI.Bonuses;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Managers
{
    public class BonusManager
    {
        private readonly ChronaManager _chronaManager;
        
        private BonusPanel _bonusPanel;
        private AllyManager _allyManager;

        public BonusManager(ChronaManager chronaManager, AllyManager allyManager)
        {
            _chronaManager = chronaManager;
            _allyManager = allyManager;
        }

        public void SetPanel(BonusPanel bonusPanel) => _bonusPanel = bonusPanel;

        public async UniTask WaitForPlayerChoice(BonusDatabase database)
        {
            BonusData bonus;

            if (!_chronaManager.IsChronaInParty())  //Хрона ще не в паті
            {
                bonus = ReturnChrona(database);

                if (_chronaManager.IsFirstEncounter())
                {
                    Debug.Log("TODO: діалог першої зустрічі");
                    _chronaManager.RescueChrona();
                }
                else
                {
                    Debug.Log("TODO: діалог другої зустрічі");
                }
            }
            else //Хрона вже в паті
            {
                bonus = GetRandomBonus(database);
            }
            
            if (!bonus) return;
            
            var token = new UniTaskCompletionSource();
            _bonusPanel.Show(bonus, () => token.TrySetResult());
            await token.Task;
            if (bonus._type == BonusType.NewAlly)
                _allyManager.AddAlly(bonus.allyPrefab);
            //TODO: switch/case якщо NewAlly, якщо Money, якщо SessionBuff
        }

        private BonusData GetRandomBonus(BonusDatabase database)
        {
            var availableBonuses = database.allBonuses.
                Where(b => b._type != BonusType.NewAlly ||
                                     _allyManager.HasFreeSlot())
                .ToArray();
            if (availableBonuses.Length == 0)
            {
                Debug.LogWarning("Пул бонусів порожній або не відповідає умовам!");
                return null;
            }
            return availableBonuses[Random.Range(0, availableBonuses.Length)];
        }

        private BonusData ReturnChrona(BonusDatabase database)
        {
            return database.allBonuses.
                FirstOrDefault(b => b._type == BonusType.NewAlly && 
                                              b.allyPrefab != null && 
                                              b.allyPrefab.name == "Chrona");
        }
    }
}