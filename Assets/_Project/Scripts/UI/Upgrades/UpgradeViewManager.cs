using System.Collections.Generic;
using _Project.Scripts.Managers;
using _Project.Scripts.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Upgrades
{
    public class UpgradeViewManager : MonoBehaviour
    {
        [SerializeField] private UpgradeButton[] _upgradeButtons;
        [SerializeField] private List<UpgradeData> _allUpgrades;
        [SerializeField] private CharacterSelectPanel _characterSelectPanel;
        
        private CurrencyManager _currencyManager;
        private AllyManager _allyManager;

        [Inject]
        public void Construct(CurrencyManager currencyManager, AllyManager allyManager)
        {
            _currencyManager = currencyManager;
            _allyManager = allyManager;
        }

        private void OnEnable()
        {
            WriteDataToUpgradeButtons();
        }
        
        private List<UpgradeData> GetRandomUpgrades()
        {
            List<UpgradeData> randomUpgrades = new List<UpgradeData>();
            List<UpgradeData> tempList = new List<UpgradeData>(_allUpgrades);
    
            for (int i = 0; i < _upgradeButtons.Length; i++)
            {
                int randomIndex = Random.Range(0, tempList.Count);
                randomUpgrades.Add(tempList[randomIndex]);
                tempList.RemoveAt(randomIndex);
            }
            
            return randomUpgrades;
        }

        private void WriteDataToUpgradeButtons()
        {
            var randomUpgrades = GetRandomUpgrades();
            for (int i = 0; i < _upgradeButtons.Length; i++)
            {
                _upgradeButtons[i].SetButtonData(randomUpgrades[i]);
                _upgradeButtons[i].OnClicked -= OnUpgradeSelected;
                _upgradeButtons[i].OnClicked += OnUpgradeSelected;
            }
        }
        
        private void OnUpgradeSelected(UpgradeData data)
        {
            if (data.rewardType == RewardType.Money)
            {
                _currencyManager.Add((int)data.value);
                gameObject.SetActive(false);
                return;
            }
            _characterSelectPanel.Show(data, OnCharacterChosen); // 🟢 відкриваємо панель вибору героя
        }
        
        private void OnCharacterChosen(Units unit, UpgradeData data)
        {
            var unitController = _allyManager.GetUnitByType(unit);
            if (unitController != null)
                unitController.ApplyUpgrade(data);
            else
                Debug.LogWarning($"Юніт {unit} не знайдений!");
            gameObject.SetActive(false);
        }
    }
}