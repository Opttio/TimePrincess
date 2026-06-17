using _Project.Scripts.Runtime.Characters;
using UnityEngine;

namespace _Project.Scripts.ScriptableObjects
{
    public enum BonusType
    {
        NewAlly,
        Money,
        SessionBuff
    }
    
    [CreateAssetMenu(fileName = "BonusData", menuName = "Bonus/BonusData", order = 0)]
    public class BonusData : ScriptableObject
    {
        public BonusType _type;
        public string description;
        public Sprite icon;
        
        [Header ("NewAlly")]
        public UnitController allyPrefab;

        [Header("Money")] 
        public int amount;

        [Header("SessionBuff")]
        public UpgradeData UpgradeData;
    }
}