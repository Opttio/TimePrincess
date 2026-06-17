using _Project.Scripts.Runtime.Characters;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace _Project.Scripts.UI.MainGameUi
{
    public class UnitBarView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _characterIcon;
        [SerializeField] private Button[] _skillButtons; // Масив кнопок для скілів

        private UnitController _unit;

        public void Bind(UnitController unit)
        {
            if (_unit == unit)
                return;
            if (_unit != null)
            {
                _unit.OnSkillCooldownUpdatedForUI -= UpdateSkillUI;
                _unit.OnHealthChanged -= UpdateHealthBar;
            }
            
            _unit = unit ?? throw new ArgumentNullException(nameof(unit));
            
            // Іконка персонажа
            if (_unit.MyUnitData && _characterIcon)
                _characterIcon.sprite = _unit.MyUnitData.unitIcon;

            // Підписка на кнопки скілів
            for (int i = 0; i < _skillButtons.Length; i++)
            {
                int skillIndex = i; // локальна копія для замикання
                if (_skillButtons[i] != null)
                {
                    _skillButtons[i].onClick.RemoveAllListeners();
                    _skillButtons[i].onClick.AddListener(() => OnSkillClicked(skillIndex));
                }
            }

            // Підписка на події з UnitController
            _unit.OnSkillCooldownUpdatedForUI += UpdateSkillUI;
            _unit.OnHealthChanged += UpdateHealthBar;

            // Початкове оновлення UI
            UpdateHealthBar(_unit.CurrentHealth, _unit.MaxHealth);
            UpdateAllSkillUI();
        }

        private void OnDestroy()
        {
            if (_unit != null)
            {
                _unit.OnSkillCooldownUpdatedForUI -= UpdateSkillUI;
                _unit.OnHealthChanged -= UpdateHealthBar;
            }

            if (_skillButtons != null)
            {
                foreach (var btn in _skillButtons)
                    btn.onClick.RemoveAllListeners();
            }
        }

        private void OnSkillClicked(int skillIndex)
        {
            _unit.ReduceCooldown(skillIndex);
        }

        private void UpdateSkillUI(int skillIndex, float fill)
        {
            if (_skillButtons == null || skillIndex < 0 || skillIndex >= _skillButtons.Length)
                return;

            var btn = _skillButtons[skillIndex];
            if (btn != null && btn.image != null)
                btn.image.fillAmount = Mathf.Clamp01(fill);
        }

        private void UpdateAllSkillUI()
        {
            if (_unit == null || _skillButtons == null)
                return;

            for (int i = 0; i < _skillButtons.Length; i++)
            {
                var btn = _skillButtons[i];
                if (btn != null)
                {
                    var skillRuntime = _unit.GetSkillRuntime(i);
                    if (skillRuntime != null && skillRuntime.rawCooldown > 0f)
                    {
                        btn.gameObject.SetActive(true);
                        btn.image.fillAmount = 1f - (skillRuntime.cooldownRemaining / skillRuntime.rawCooldown);
                    }
                    else
                    {
                        btn.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void UpdateHealthBar(int currentHp, int maxHp)
        {
            if (_healthBar != null && maxHp > 0)
                _healthBar.fillAmount = Mathf.Clamp01((float)currentHp / maxHp);
        }
    }
}