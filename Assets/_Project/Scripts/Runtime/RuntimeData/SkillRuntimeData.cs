using _Project.Scripts.ScriptableObjects;

namespace _Project.Scripts.Runtime.RuntimeData
{
    [System.Serializable]
    public class SkillRuntimeData
    {
        public int skillIndex;
        public string skillName;
        public int value;
        public float rawCooldown;
        public float cooldownRemaining;
        public SkillType skillType;
    }
}