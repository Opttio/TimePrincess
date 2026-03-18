using _Project.Scripts.Core;
using _Project.Scripts.Managers;
using _Project.Scripts.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Runtime.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private AllyManager _allyManager;
        [SerializeField] private UnitsDatabase _unitsDatabase;
        public override void InstallBindings()
        {
            // EventBus живе весь час гри
            Container.Bind<GameEventBus>().AsSingle().NonLazy();
            Container.Bind<AllyManager>().FromInstance(_allyManager).AsSingle();
            Container.Bind<UnitsDatabase>().FromInstance(_unitsDatabase).AsSingle();
        }
        
    }
}