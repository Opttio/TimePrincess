using System;
using _Project.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Upgrades
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _upgradeName;
        [SerializeField] private TMP_Text _upgradeDescription;
        [SerializeField] private Button _button;
        private UpgradeData _buttonData;
        public event Action<UpgradeData> OnClicked;

        private void OnEnable() => _button.onClick.AddListener(SendUpgrade);
        private void OnDisable() => _button.onClick.RemoveListener(SendUpgrade);

        private void SendUpgrade()
        {
            OnClicked?.Invoke(_buttonData);
        }

        public void SetButtonData(UpgradeData upgradeData)
        {
            _iconImage.sprite = upgradeData.icon;
            _upgradeName.text = upgradeData.name;
            _upgradeDescription.text = upgradeData.description;
            _buttonData = upgradeData;
        }
    }
}