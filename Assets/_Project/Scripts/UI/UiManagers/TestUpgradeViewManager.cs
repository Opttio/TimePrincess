using UnityEngine;

namespace _Project.Scripts.UI.UiManagers
{
    public class TestUpgradeViewManager : MonoBehaviour
    {
        [SerializeField] private TestUIManager _uiManager;

        // Викликається, коли рівень закінчено
        public void ShowUpgrades()
        {
            _uiManager.ShowUpgradeUI();
        }

        // Викликається UpgradeViewManager, коли гравець вибрав юніта
        public void OnUnitChosen()
        {
            _uiManager.HideAllUI();
        }
    }
}