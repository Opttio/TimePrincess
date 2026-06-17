using UnityEngine;

namespace _Project.Scripts.ScriptableObjects
{
    public enum UpgradeType
    {
        FirstSkillDamageInPercent,
        FirstSkillDamageInTerms,
        FirstSkillCooldownInPercent,
        SecondSkillDamageInPercent,
        SecondSkillDamageInTerms,
        SecondSkillCooldownInPercent,
        MaxHealthInPercent,
        MaxHealthInTerms,
        ArmorInPercent,
        ArmorInTerms
    }

    public enum RewardType
    {
        Upgrade,
        Money
    }
    
    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData", order = 0)]
    public class UpgradeData : ScriptableObject
    {
        public string upgradeName;
        [TextArea] public string description;
        public Sprite icon;
        public RewardType rewardType;
        [Tooltip("Використовується тільки якщо RewardType == Upgrade")] public UpgradeType type;
        public float value;
    }
}