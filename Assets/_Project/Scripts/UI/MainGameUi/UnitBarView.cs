using _Project.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.MainGameUi
{
    public class UnitBarView : MonoBehaviour
    {
        [SerializeField] private Button _firstSkillButton;
        [SerializeField] private Button _secondSkillButton;
        
        private GameEventBus _eventBus;

        [Inject]
        public void Construct(GameEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _firstSkillButton.onClick.AddListener(ReduceFirstSkillCooldown);
            _secondSkillButton.onClick.AddListener(ReduceSecondSkillCooldown);
            
            _eventBus.OnSkillCooldownUpdate += UpdateSkillUI;
        }

        private void OnDisable()
        {
            _firstSkillButton.onClick.RemoveListener(ReduceFirstSkillCooldown);
            _secondSkillButton.onClick.RemoveListener(ReduceSecondSkillCooldown);
            
            _eventBus.OnSkillCooldownUpdate -= UpdateSkillUI;
        }

        private void ReduceFirstSkillCooldown()
        {
            _eventBus.SignalToReduceSkill(0);
        }

        private void ReduceSecondSkillCooldown()
        {
            _eventBus.SignalToReduceSkill(1);
        }
        
        private void UpdateSkillUI(int index, float fill)
        {
            if (index == 0)
                _firstSkillButton.image.fillAmount = fill;
            else if (index == 1)
                _secondSkillButton.image.fillAmount = fill;
        }
    }
}