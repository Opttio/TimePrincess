using _Project.Scripts.Runtime.RuntimeData;
using _Project.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Project.Scripts.Runtime.Systems
{
    public class UnitStatsSystem
    {
        private UnitsData _base;
        private UnitStatsData _session;
        private UnitStatsData _permanent;
        private const float CooldownCap = 80f;

        public UnitStatsSystem(UnitsData baseData, UnitStatsData permanent)
        {
            _base = baseData;
            _permanent = permanent;
            _session = new UnitStatsData();
        }
        
        public int GetMaxHealth() => _base.maxHealth + _session.bonusHealth + _permanent.bonusHealth;
        
        public float GetArmor() => _base.armor + _session.bonusArmour + _permanent.bonusArmour;

        public int GetSkillValue(int skillIndex)
        {
            int baseValue = _base.skillData[skillIndex].value;

            int sessionBonus = skillIndex == 0
                ? _session.bonusSkill0Value
                : _session.bonusSkill1Value;
            
            int permanentBonus = skillIndex == 0
                ? _permanent.bonusSkill0Value
                : _permanent.bonusSkill1Value;
            
            return baseValue + sessionBonus + permanentBonus;
        }
        
        public float GetSkillCooldown(int skillIndex)
        {
            float baseCooldown = _base.skillData[skillIndex].cooldown;

            float sessionPercent = skillIndex == 0
                ? _session.bonusSkill0CooldownPercent
                : _session.bonusSkill1CooldownPercent;
            
            float permanentPercent = skillIndex == 0
                ? _permanent.bonusSkill0CooldownPercent
                : _permanent.bonusSkill1CooldownPercent;
            
            float totalPercent = sessionPercent + permanentPercent;
            return baseCooldown * (1f - totalPercent / 100f);
        }

        public void ApplyUpgrade(UpgradeData upgrade) => Apply(upgrade, _session);
        
        public void ApplyPermanentUpgrade(UpgradeData upgrade) => Apply(upgrade, _permanent);
        
        public void ResetSession() => _session = new UnitStatsData();

        private void Apply(UpgradeData upgrade, UnitStatsData target)
        {
            switch (upgrade.type)
            {
                case UpgradeType.MaxHealthInTerms:
                    target.bonusHealth += Mathf.RoundToInt(upgrade.value);
                    break;
                case UpgradeType.MaxHealthInPercent:
                    target.bonusHealth += Mathf.RoundToInt(_base.maxHealth * upgrade.value / 100f);
                    break;
                case UpgradeType.ArmorInTerms:
                    target.bonusArmour += upgrade.value;
                    break;
                case UpgradeType.ArmorInPercent:
                    target.bonusArmour += _base.armor * upgrade.value / 100f;
                    break;
                case UpgradeType.FirstSkillValueInTerms:
                    target.bonusSkill0Value += Mathf.RoundToInt(upgrade.value);
                    break;
                case UpgradeType.FirstSkillValueInPercent:
                    target.bonusSkill0Value += Mathf.RoundToInt(_base.skillData[0].value * upgrade.value / 100f);
                    break;
                case UpgradeType.FirstSkillCooldownInPercent:
                    target.bonusSkill0CooldownPercent += upgrade.value;
                    target.bonusSkill0CooldownPercent = Mathf.Min(target.bonusSkill0CooldownPercent, CooldownCap);
                    break;
                case UpgradeType.SecondSkillValueInTerms:
                    target.bonusSkill1Value += Mathf.RoundToInt(upgrade.value);
                    break;
                case UpgradeType.SecondSkillValueInPercent:
                    target.bonusSkill1Value += Mathf.RoundToInt(_base.skillData[1].value * upgrade.value / 100f);
                    break;
                case UpgradeType.SecondSkillCooldownInPercent:
                    target.bonusSkill1CooldownPercent += upgrade.value;
                    target.bonusSkill1CooldownPercent = Mathf.Min(target.bonusSkill1CooldownPercent, CooldownCap);
                    break;
            }
        }
    }
}