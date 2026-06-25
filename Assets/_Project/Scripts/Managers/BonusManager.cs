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
        private readonly DialogueManager _dialogueManager;
        private readonly EncounterDatabase _encounterDatabase;
        
        private BonusPanel _bonusPanel;
        private AllyManager _allyManager;
        private CurrencyManager _currencyManager;

        public BonusManager(ChronaManager chronaManager, AllyManager allyManager, CurrencyManager currencyManager,
                            DialogueManager dialogueManager, EncounterDatabase encounterDatabase)
        {
            _chronaManager = chronaManager;
            _allyManager = allyManager;
            _currencyManager = currencyManager;
            _dialogueManager = dialogueManager;
            _encounterDatabase = encounterDatabase;
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
                    await _dialogueManager.PlayDialogue(_encounterDatabase.chronaFirstDialogue);
                    _chronaManager.RescueChrona();
                }
                else
                {
                    Debug.Log("TODO: діалог другої зустрічі");
                    // TODO: await _dialogueManager.PlayDialogue(_encounterDatabase.chronaSecondDialogue);
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
            
            switch (bonus._type)
            {
                case BonusType.NewAlly:
                    _allyManager.AddAlly(bonus.allyPrefab);
                    break;
                case BonusType.Money:
                    _currencyManager.Add(bonus.amount);
                    break;
                case BonusType.Heal:
                    foreach (var unit in _allyManager.GetAllUnits()) 
                        unit.Heal(bonus.healAmount);
                    break;
                case BonusType.SessionBuff:
                    if (bonus.UpgradeData)
                        _allyManager.ApplyBonusUpgradeToAll(bonus.UpgradeData);
                    else
                        Debug.LogWarning("Не підключено або нема SessionBuffData");
                    break;
            }
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