using System.Collections.Generic;
using _Project.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Project.Scripts.UI.Upgrades
{
    public class UpgradeViewManager : MonoBehaviour
    {
        [SerializeField] private UpgradeButton[] _upgradeButtons;
        [SerializeField] private List<UpgradeData> _allUpgrades;
        [SerializeField] private CharacterSelectPanel _characterSelectPanel;

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
                _upgradeButtons[i].OnClicked += OnUpgradeSelected;
            }
        }
        
        private void OnUpgradeSelected(UpgradeData data)
        {
            _characterSelectPanel.Show(data, OnCharacterChosen); // 🟢 відкриваємо панель вибору героя
        }
        
        private void OnCharacterChosen(Units unit, UpgradeData data)
        {
            // 🟢 Тут викличеш UnitStatsManager або щось схоже
            Debug.Log($"Chosen {data.upgradeName} for {unit}");
            // ApplyUpgrade(unit, data);
        }
    }
}