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

        public int GetSkillDamage(int skillIndex)
        {
            int baseDamage = _base.skillData[skillIndex].damage;

            int sessionBonus = skillIndex == 0
                ? _session.bonusSkill0Damage
                : _session.bonusSkill1Damage;
            
            int permanentBonus = skillIndex == 0
                ? _permanent.bonusSkill0Damage
                : _permanent.bonusSkill1Damage;
            
            return baseDamage + sessionBonus + permanentBonus;
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
                case UpgradeType.FirstSkillDamageInTerms:
                    target.bonusSkill0Damage += Mathf.RoundToInt(upgrade.value);
                    break;
                case UpgradeType.FirstSkillDamageInPercent:
                    target.bonusSkill0Damage += Mathf.RoundToInt(_base.skillData[0].damage * upgrade.value / 100f);
                    break;
                case UpgradeType.FirstSkillCooldownInPercent:
                    target.bonusSkill0CooldownPercent += upgrade.value;
                    target.bonusSkill0CooldownPercent = Mathf.Min(target.bonusSkill0CooldownPercent, CooldownCap);
                    break;
                case UpgradeType.SecondSkillDamageInTerms:
                    target.bonusSkill1Damage += Mathf.RoundToInt(upgrade.value);
                    break;
                case UpgradeType.SecondSkillDamageInPercent:
                    target.bonusSkill1Damage += Mathf.RoundToInt(_base.skillData[1].damage * upgrade.value / 100f);
                    break;
                case UpgradeType.SecondSkillCooldownInPercent:
                    target.bonusSkill1CooldownPercent += upgrade.value;
                    target.bonusSkill1CooldownPercent = Mathf.Min(target.bonusSkill1CooldownPercent, CooldownCap);
                    break;
            }
        }
    }
}