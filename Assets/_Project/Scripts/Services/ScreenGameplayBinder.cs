using _Project.Scripts.Managers;
using _Project.Scripts.Runtime.Environment;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services
{
    public class ScreenGameplayBinder : MonoBehaviour
    {
        private AllyManager _allyManager;
        private EnemyManager _enemyManager;
        private BonusManager _bonusManager;

        [Inject]
        public void Construct(
            AllyManager allyManager,
            EnemyManager enemyManager,
            BonusManager bonusManager)
        {
            _allyManager = allyManager;
            _enemyManager = enemyManager;
            _bonusManager = bonusManager;
        }
        
        public void Bind(ScreenView screen)
        {
            _allyManager.SetActiveScreenSlots(screen.AllySlots);
            _allyManager.ReassignAllies();

            _enemyManager.SetActiveSlots(screen.EnemySlots);

            _bonusManager.GetRandomBonus();
        }
    }
}