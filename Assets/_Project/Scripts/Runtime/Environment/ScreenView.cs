using UnityEngine;

namespace _Project.Scripts.Runtime.Environment
{
    public class ScreenView : MonoBehaviour
    {
        [SerializeField] private Transform _cameraPoint;
        [SerializeField] private AllySlot[] _allySlots;
        [SerializeField] private EnemySlot[] _enemySlots;
        [SerializeField] private BonusSlot _bonusSlot;
        
        public Transform CameraPoint => _cameraPoint;
        public AllySlot[] AllySlots => _allySlots;
        public EnemySlot[] EnemySlots => _enemySlots;
        public BonusSlot BonusSlot => _bonusSlot;
    }
}