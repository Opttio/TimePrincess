using UnityEngine;

namespace _Project.Scripts.ScriptableObjects
{
    public enum Units
    {
        KnightStelis,
        PrincessChrona,
        MageBalis,
        BasicWarrior,
        BasicArcher
    }
    
    [CreateAssetMenu(fileName = "UnitData", menuName = "Units/UnitData", order = 0)]
    public class UnitsData : ScriptableObject
    {
        [Header("Base Info")]
        public string unitName;
        public Units unitType;
        public Sprite unitIcon; // 🟢 нове поле для іконки в UI

        [Header("Stats")]
        public int maxHealth;
        public float armor;
        public float clickCooldownReductionPercent;

        [Header("Skills")]
        public SkillData[] skillData;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            // Автокламп значень у Inspector і лог-повідомлення
            if (maxHealth < 1)
            {
                Debug.LogWarning($"{name}: maxHealth was < 1 — clamped to 1.", this);
                maxHealth = Mathf.Max(1, maxHealth);
            }

            if (armor < 0)
            {
                Debug.LogWarning($"{name}: armor < 0 — clamped to 0.", this);
                armor = Mathf.Max(0f, armor);
            }

            if (skillData != null)
            {
                for (int i = 0; i < skillData.Length; i++)
                {
                    if (skillData[i].damage < 0)
                    {
                        Debug.LogWarning($"{name}: skill '{skillData[i].skillName}' damage < 0 — clamped to 0.", this);
                        skillData[i].damage = Mathf.Max(0, skillData[i].damage);
                    }
                    if (skillData[i].cooldown < 0f)
                    {
                        Debug.LogWarning($"{name}: skill '{skillData[i].skillName}' cooldown < 0 — clamped to 0.", this);
                        skillData[i].cooldown = Mathf.Max(0f, skillData[i].cooldown);
                    }
                }
            }
        }
#endif
    }
    
    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public int damage;
        public float cooldown;
    }
    
    
}