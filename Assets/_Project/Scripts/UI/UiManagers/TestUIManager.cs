using _Project.Scripts.Managers;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.UiManagers
{
    public class TestUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _upgradeViewManagerGO;
        [SerializeField] private GameObject _characterSelectPanelGO;
        [SerializeField] private ScreenManager _screenManager;

        private void Start()
        {
            // Спочатку приховуємо UI
            _upgradeViewManagerGO.SetActive(false);
            _characterSelectPanelGO.SetActive(false);
        }

        /// <summary>
        /// Викликається після перемоги на Screen
        /// </summary>
        public void ShowUpgradeUI()
        {
            _upgradeViewManagerGO.SetActive(true);
        }

        /// <summary>
        /// Викликається UpgradeViewManager коли треба вибрати юніта
        /// </summary>
        public void ShowCharacterSelect()
        {
            _characterSelectPanelGO.SetActive(true);
        }

        /// <summary>
        /// Викликається після вибору юніта
        /// </summary>
        public void HideAllUI()
        {
            _upgradeViewManagerGO.SetActive(false);
            _characterSelectPanelGO.SetActive(false);
        }
    }
}