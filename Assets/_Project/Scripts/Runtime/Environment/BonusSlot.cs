using _Project.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Project.Scripts.Runtime.Environment
{
    public class BonusSlot : MonoBehaviour
    {
        [SerializeField] private BonusDatabase _database;
        
        public BonusDatabase Database => _database;

        public bool IsEnabled()
        {
            return gameObject.activeSelf && _database != null;
        }
    }
}