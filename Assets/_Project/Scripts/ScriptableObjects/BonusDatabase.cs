using UnityEngine;
using Zenject;

namespace _Project.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BonusDatabase", menuName = "Bonus/BonusDatabase")]
    public class BonusDatabase : ScriptableObjectInstaller<BonusDatabase>
    {
        public BonusData[] allBonuses;

        public override void InstallBindings() => Container.BindInstance(this).AsSingle();
    }
}