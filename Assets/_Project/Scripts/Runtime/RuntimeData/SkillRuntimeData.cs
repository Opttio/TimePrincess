namespace _Project.Scripts.Runtime.RuntimeData
{
    [System.Serializable]
    public class SkillRuntimeData
    {
        public int skillIndex;
        public string skillName;
        public int damage;
        public float rawCooldown;
        public float cooldownRemaining;
    }
}