using _Project.Scripts.Runtime.Characters;
using UnityEngine;

namespace _Project.Scripts.Tests
{
    public class TestDamageButton : MonoBehaviour
    {
        [SerializeField] private UnitController[] _allies;
        [SerializeField] private int _damage = 7;
        [SerializeField] private KeyCode _key = KeyCode.Alpha7;

        private void Update()
        {
            if (Input.GetKeyDown(_key))
            {
                foreach (var u in _allies)
                {
                    u.DebugTakeDamage(_damage);
                }
            }
        }
    }
}