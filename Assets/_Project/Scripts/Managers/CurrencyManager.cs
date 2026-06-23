using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class CurrencyManager : MonoBehaviour
    {
        private const string SaveKey = "PlayerMoney";
        private int _amount;
        public int Amount => _amount;

        private void Awake()
        {
            _amount = PlayerPrefs.GetInt(SaveKey, 0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
                Debug.Log($"PlayerMoney: {_amount}");
            if (Input.GetKeyDown(KeyCode.R))
                ResetProgress();
        }

        public void Add(int amount)
        {
            _amount += amount;
            PlayerPrefs.SetInt(SaveKey, _amount);
        }

        public bool TrySpend(int amount)
        {
            if (_amount < amount) return false;
            _amount -= amount;
            PlayerPrefs.SetInt(SaveKey, _amount);
            return true;
        }
        
        public void ResetProgress()
        {
            _amount = 0;
            PlayerPrefs.DeleteKey(SaveKey);
        }
    }
}