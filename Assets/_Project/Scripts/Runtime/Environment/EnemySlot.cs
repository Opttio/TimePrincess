using _Project.Scripts.Runtime.Characters;
using UnityEngine;

namespace _Project.Scripts.Runtime.Environment
{
    public class EnemySlot : UnitSlot
    {
        private void Awake()
        {
            var unit = GetComponentInChildren<UnitController>();
            if (unit != null)
                SetUnit(unit);
        }
    }
}