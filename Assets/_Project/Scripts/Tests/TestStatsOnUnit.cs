using _Project.Scripts.Managers;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Tests
{
    public class TestStatsOnUnit : MonoBehaviour
    {
        private AllyManager _allyManager;
        
        [Inject]
        public void Construct(AllyManager allyManager)
        {
            _allyManager = allyManager;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                PrintAllStats();
        }

        private void PrintAllStats()
        {
            Debug.Log("------UNIT STATS------");
            foreach (var unit in _allyManager.GetAllUnits())
            {
                Debug.Log($"[{unit.MyUnitData.unitName}] + MaxHP: {unit.CurrentHealth} + Armor: {unit.CurrentArmor}");
            }
        }
    }
}