using _Project.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Project.Scripts.Runtime.Environment
{
    public class BonusSlot : MonoBehaviour
    {
        public BonusData CurrentBonus {get; private set;}

        public void SetBonus(BonusData bonus)
        {
            CurrentBonus = bonus;
        }

        public void Clear()
        {
            CurrentBonus = null;
        }
    }
}