using System.Collections.Generic;
using _Project.Scripts.Runtime.Environment;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class ScreenRegistry : MonoBehaviour
    {
        [SerializeField] private Transform _levelRoot;
        
        private List<ScreenView> _screens = new();
        private int _currentIndex;
        
        public ScreenView CurrentScreen => _screens[_currentIndex];
        public IReadOnlyList<ScreenView> Screens => _screens; 
        
        private void Awake()
        {
            _screens = new List<ScreenView>(_levelRoot.GetComponentsInChildren<ScreenView>(true));
            _screens.Sort((a, b) => a.name.CompareTo(b.name));
        }

        public bool HasNextScreen()
        {
            return _currentIndex < _screens.Count - 1;
        }

        public ScreenView GoNext()
        {
            _currentIndex++;
            return CurrentScreen;
        }
    }
}