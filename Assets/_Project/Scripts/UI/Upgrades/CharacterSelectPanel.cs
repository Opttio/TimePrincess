using System;
using _Project.Scripts.Managers;
using _Project.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Upgrades
{
    public class CharacterSelectPanel : MonoBehaviour
    {
        [SerializeField] private Button[] _unitButtons;
        [SerializeField] private Image[] _unitIcons;

        private Action<Units, UpgradeData> _onChosen;
        private UpgradeData _pendingUpgrade;
        
        private AllyManager _allyManager;
        private UnitsDatabase _unitsDatabase;

        [Inject]
        public void Construct(AllyManager allyManager, UnitsDatabase unitsDatabase)
        {
            _allyManager = allyManager;
            _unitsDatabase = unitsDatabase;
        }

        private void OnDisable()
        {
            foreach (var button in _unitButtons)
                button.onClick.RemoveAllListeners();
        }
        
        public void Show(UpgradeData upgradeData, Action<Units, UpgradeData> onChosen)
        {
            _pendingUpgrade = upgradeData;
            _onChosen = onChosen;

            // Отримуємо список активних юнітів з AllyManager
            var activeUnits = _allyManager.GetActiveUnitTypes();

            for (int i = 0; i < _unitButtons.Length; i++)
            {
                if (i < activeUnits.Count)
                {
                    var unitType = activeUnits[i];
                    var unitData = _unitsDatabase.GetData(unitType);

                    // Вмикаємо кнопку та встановлюємо іконку
                    _unitButtons[i].gameObject.SetActive(true);
                    if (unitData)
                        _unitIcons[i].sprite = unitData.unitIcon;

                    int index = i; // для замикання
                    _unitButtons[i].onClick.RemoveAllListeners();
                    _unitButtons[i].onClick.AddListener(() => ChooseUnit(activeUnits[index]));
                }
                else
                {
                    // Якщо юнітів менше ніж кнопок — ховаємо кнопку
                    _unitButtons[i].gameObject.SetActive(false);
                }
            }

            gameObject.SetActive(true);
        }

        private void ChooseUnit(Units unit)
        {
            _onChosen?.Invoke(unit, _pendingUpgrade);
            
            _pendingUpgrade = null;
            gameObject.SetActive(false);
        }
    }
}