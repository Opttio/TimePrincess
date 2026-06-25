using _Project.Scripts.Core;
using _Project.Scripts.Managers;
using _Project.Scripts.Meta.MetaData;
using _Project.Scripts.Runtime.RuntimeData;
using _Project.Scripts.ScriptableObjects;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Upgrades;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Runtime.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private AllyManager _allyManager;
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private CombatManager _combatManager;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private ScreenManager _screenManager;
        [SerializeField] private ScreenRegistry _screenRegistry;
        [SerializeField] private ScreenTransitionService _screenTransitionService;
        [SerializeField] private ScreenGameplayBinder _screenGameplayBinder;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private UnitsDatabase _unitsDatabase;
        [SerializeField] private UpgradeViewManager _upgradeViewManager;
        [SerializeField] private CurrencyManager _currencyManager;
        
            
        public override void InstallBindings()
        {
            // EventBus живе весь час гри
            Container.Bind<GameEventBus>().AsSingle().NonLazy();
            
            Container.Bind<RunData>().AsSingle();
            Container.Bind<MetaProgressData>().AsSingle();
            Container.Bind<ChronaManager>().AsSingle();
            Container.Bind<BonusManager>().AsSingle();
            Container.Bind<DialogueManager>().AsSingle();
            
            Container.Bind<AllyManager>().FromInstance(_allyManager).AsSingle();
            Container.Bind<CameraManager>().FromInstance(_cameraManager).AsSingle();
            Container.Bind<CombatManager>().FromInstance(_combatManager).AsSingle();
            Container.Bind<EnemyManager>().FromInstance(_enemyManager).AsSingle();
            Container.Bind<ScreenManager>().FromInstance(_screenManager).AsSingle();
            Container.Bind<ScreenRegistry>().FromInstance(_screenRegistry).AsSingle();
            Container.Bind<ScreenTransitionService>().FromInstance(_screenTransitionService).AsSingle();
            Container.Bind<ScreenGameplayBinder>().FromInstance(_screenGameplayBinder).AsSingle();
            Container.Bind<BattleManager>().FromInstance(_battleManager).AsSingle();
            Container.Bind<UnitsDatabase>().FromInstance(_unitsDatabase).AsSingle();
            Container.Bind<UpgradeViewManager>().FromInstance(_upgradeViewManager).AsSingle();
            Container.Bind<CurrencyManager>().FromInstance(_currencyManager).AsSingle();
        }
        
    }
}