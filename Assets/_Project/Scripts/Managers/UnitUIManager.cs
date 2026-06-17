using _Project.Scripts.UI.MainGameUi;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Managers
{
    public class UnitUIManager : MonoBehaviour
    {
        [SerializeField] private UnitBarView _unitBarPrefab;
        [SerializeField] private Transform[] _uiSlots;
        
        private AllyManager _allyManager;
        private UnitBarView[] _spawnedViews;
        private DiContainer _container;
        
        [Inject]
        public void Construct(AllyManager allyManager, DiContainer container)
        {
            _allyManager = allyManager;
            _container = container;
            _allyManager.OnAlliesReassigned += RefreshUI;
        }

        private void OnDestroy()
        {
            if (_allyManager != null) 
                _allyManager.OnAlliesReassigned -= RefreshUI;
        }

        public void RefreshUI()
        {
            Clear();
            _spawnedViews = new UnitBarView[_uiSlots.Length];
            for (int i = 0; i < _uiSlots.Length; i++)
            {
                var unit = _allyManager.GetUnitInSlot(i);
                if (!unit)
                    continue;
                var view = _container.InstantiatePrefabForComponent<UnitBarView>(_unitBarPrefab.gameObject, _uiSlots[i]);
                view.Bind(unit);
                _spawnedViews[i] = view;
            }
        }

        private void Clear()
        {
            if(_spawnedViews == null) return;
            foreach (var view in _spawnedViews)
            {
                if (view != null) 
                    Destroy(view.gameObject);
            }
        }
    }
}