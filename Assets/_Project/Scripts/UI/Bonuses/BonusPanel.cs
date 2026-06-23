using System;
using _Project.Scripts.Managers;
using _Project.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Bonuses
{
    public class BonusPanel : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _acceptButton;

        private Action _onAccepted;
        
        private BonusManager _bonusManager;

        [Inject]
        public void Construct(BonusManager bonusManager)
        {
            _bonusManager = bonusManager;
            _bonusManager.SetPanel(this);
        }

        private void OnEnable() => _acceptButton.onClick.AddListener(OnAcceptClicked);
        private void OnDisable() => _acceptButton.onClick.RemoveListener(OnAcceptClicked);

        public void Show(BonusData bonusData, Action onAccepted)
        {
            _onAccepted = onAccepted;
            _icon.sprite = bonusData.icon;
            _description.text = bonusData.description;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnAcceptClicked()
        {
            Hide();
            _onAccepted?.Invoke();
        }
    }
}