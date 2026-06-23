using _Project.Scripts.Core;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Upgrades;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Managers
{
    public class ScreenManager : MonoBehaviour
    {
        private ScreenRegistry _registry;
        private ScreenTransitionService _transition;
        private ScreenGameplayBinder _binder;
        private CameraManager _cameraManager;
        private BattleManager _battleManager;
        private UpgradeViewManager _upgradeViewManager;
        private GameEventBus _eventBus;
        private BonusManager _bonusManager;

        [Inject]
        public void Construct(
            ScreenRegistry registry,
            ScreenTransitionService transition,
            ScreenGameplayBinder binder,
            CameraManager cameraManager,
            BattleManager battleManager,
            UpgradeViewManager upgradeViewManager,
            GameEventBus eventBus,
            BonusManager bonusManager)
        {
            _registry = registry;
            _transition = transition;
            _binder = binder;
            _cameraManager = cameraManager;
            _battleManager = battleManager;
            _upgradeViewManager = upgradeViewManager;
            _eventBus = eventBus;
            _bonusManager = bonusManager;
        }

        private void Start()
        {
            ActivateCurrent();
            _binder.Bind(_registry.CurrentScreen);
            _battleManager.Initialize();
        }

        public async UniTaskVoid GoNextScreen()
        {
            _eventBus.BattlePause();
            var slot = _registry.CurrentScreen.BonusSlot;
            if (slot.IsEnabled())
            {
                await _bonusManager.WaitForPlayerChoice(slot.Database);
            }
            
            if (!_registry.HasNextScreen())
            {
                _upgradeViewManager.gameObject.SetActive(true);
                //TODO: Реалізувати зняття з паузи після вибору Upgrade.
                return;
            }

            await _transition.FadeAsync(1f);

            var next = _registry.GoNext();

            ActivateCurrent();

            _cameraManager.MoveTo(next.CameraPoint);

            _binder.Bind(next);
            _battleManager.Dispose();
            _battleManager.Initialize();

            await _transition.FadeAsync(0f);
            _eventBus.BattleResume();
        }

        private void ActivateCurrent()
        {
            var screens = _registry.Screens;
            var current = _registry.CurrentScreen;

            for (int i = 0; i < screens.Count; i++)
            {
                screens[i].gameObject.SetActive(screens[i] == current);
            }
        }
    }
}