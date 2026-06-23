using System;

namespace _Project.Scripts.Core
{
    public class GameEventBus
    {
        public event Action<int> OnPlayerAttack;
        public event Action<int> OnEnemyAttack;
        public event Action OnBattlePause;
        public event Action OnBattleResume;
        public event Action<int> OnPlayerHeal;
        public bool IsBattlePause { get; private set; }

        public void PlayerAttacked(int dmg) => OnPlayerAttack?.Invoke(dmg);
        public void EnemyAttacked(int dmg) => OnEnemyAttack?.Invoke(dmg);
        public void PlayerHeal(int amount) => OnPlayerHeal?.Invoke(amount);
        public void BattlePause()
        {
            IsBattlePause = true;
            OnBattlePause?.Invoke();
        }

        public void BattleResume()
        {
            IsBattlePause = false;
            OnBattleResume?.Invoke();
        }
    }
}