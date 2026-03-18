using UnityEngine;

namespace _Project.Scripts.ScriptableObjects
{
    public enum UpgradeType
    {
        FirstSkillInPercent,
        FirstSkillInTerms,
        SecondSkillInPercent,
        SecondSkillInTerms,
        MaxHealthInPercent,
        MaxHealthInTerms,
        ArmorInPercent,
        ArmorInTerms,
        CooldownInPercent,
        Money
    }
    
    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData", order = 0)]
    public class UpgradeData : ScriptableObject
    {
        public string upgradeName;
        [TextArea] public string description;
        public Sprite icon;
        public UpgradeType type;
        public float value;
    }
}