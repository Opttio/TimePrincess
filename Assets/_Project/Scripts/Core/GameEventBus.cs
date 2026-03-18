using System;

namespace _Project.Scripts.Core
{
    public class GameEventBus
    {
        public event Action<int> OnPlayerAttack;
        public event Action<int> OnEnemyAttack;
        public event Action<int> OnReduceSkillCooldown;
        public event Action<int, float> OnSkillCooldownUpdate; //це bar кнопки

        public void PlayerAttacked(int dmg) => OnPlayerAttack?.Invoke(dmg);
        public void EnemyAttacked(int dmg) => OnEnemyAttack?.Invoke(dmg);
        public void SignalToReduceSkill(int skillIndex) => OnReduceSkillCooldown?.Invoke(skillIndex);
        public void SkillCooldownUpdated(int skillIndex, float fill) => OnSkillCooldownUpdate?.Invoke(skillIndex, fill); //це bar кнопки
    }
}