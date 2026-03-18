using System.Collections.Generic;
using _Project.Scripts.UI.UiManagers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Managers
{
    public class ScreenManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _screens;       // Усі Screen-и рівня
        [SerializeField] private Transform _cameraTransform;      // Камера
        [SerializeField] private CanvasGroup _fadeCanvas;         // Canvas для затемнення
        [SerializeField] private float _fadeDuration = 1f;        // Тривалість fade
        [SerializeField] private float _moveDelay = 0.1f;         // Момент затримки між переходами
        [SerializeField] private TestUpgradeViewManager _testUpgradeViewManager;       //Тільки для тесту

        private int _currentScreenIndex = 0;
        private bool _isTransitioning = false;
        
        private AllyManager _allyManager;

        [Inject]
        public void Construct(AllyManager allyManager)
        {
            _allyManager = allyManager;
        }

        private void Awake()
        {
            _fadeCanvas.blocksRaycasts = false;
            _fadeCanvas.interactable = false;
            _fadeCanvas.alpha = 0;
        }

        private void Start()
        {
             // Увімкнути тільки перший Screen
            for (int i = 0; i < _screens.Count; i++)
                _screens[i].SetActive(i == 0);
            
            SetAllySlotsForCurrentScreen();
        }

        private void Update()
        {
            if (_isTransitioning) return;

            // Тест вручну клавішею N
            if (Input.GetKeyDown(KeyCode.N))
                _ = TryGoToNextScreenAsync();

            // Автоматична перевірка
            // if (AllEnemiesDefeatedOnCurrent())
            //     _ = TryGoToNextScreenAsync();
        }

        private bool AllEnemiesDefeatedOnCurrent()
        {
            GameObject current = _screens[_currentScreenIndex];
            foreach (Transform child in current.GetComponentsInChildren<Transform>(false))
            {
                if (child.CompareTag("Enemy") && child.gameObject.activeInHierarchy)
                    return false;
            }
            return true;
        }
        
        private Transform[] GetAllySlots(GameObject screen)
        {
            var spawnPointsParent = screen.transform.Find("Ally/SpawnPoints");
            Transform[] slots = new Transform[4];

            foreach (var t in spawnPointsParent.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("FrontUpPoint")) slots[0] = t;
                else if (t.CompareTag("FrontDownPoint")) slots[1] = t;
                else if (t.CompareTag("BackUpPoint")) slots[2] = t;
                else if (t.CompareTag("BackDownPoint")) slots[3] = t;
            }
            return slots;
        }
        
        private void SetAllySlotsForCurrentScreen()
        {
            var currentScreen = _screens[_currentScreenIndex];
            var slots = GetAllySlots(currentScreen);
            _allyManager.SetActiveScreenSlots(slots);
        }

        private async UniTaskVoid TryGoToNextScreenAsync()
        {
            if (_isTransitioning) return;
            // if (_currentScreenIndex >= _screens.Count - 1) return; // Кінець рівня
            if (_currentScreenIndex >= _screens.Count - 1)
            {
                Debug.Log("✅ Level completed! Opening upgrade UI...");
                // 🔹 Виклик тестового Upgrade UI
                FindFirstObjectByType<TestUpgradeViewManager>()?.ShowUpgrades();
                return; // зупиняємо, бо це фінал рівня
            }

            _isTransitioning = true;
            _currentScreenIndex++;

            var nextScreen = _screens[_currentScreenIndex];

            await FadeAsync(1f); // Затемнення

            // Переміщення камери
            Transform cameraPoint = nextScreen.transform.Find("CameraPosition");
            if (cameraPoint)
            {
                _cameraTransform.position = cameraPoint.position;
                _cameraTransform.rotation = cameraPoint.rotation;
            }

            // Перемикання активного Screen
            for (int i = 0; i < _screens.Count; i++)
                _screens[i].SetActive(i == _currentScreenIndex);
            
            SetAllySlotsForCurrentScreen();

            await UniTask.Delay(System.TimeSpan.FromSeconds(_moveDelay));

            await FadeAsync(0f); // Розтемнення

            _isTransitioning = false;
        }

        private async UniTask FadeAsync(float targetAlpha)
        {
            float startAlpha = _fadeCanvas.alpha;
            float time = 0f;

            // 🔹 Якщо потемнення — блокуємо кліки
            if (targetAlpha > 0f)
            {
                _fadeCanvas.blocksRaycasts = true;
                _fadeCanvas.interactable = true;
            }

            while (time < _fadeDuration)
            {
                time += Time.deltaTime;
                _fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeDuration);
                await UniTask.Yield();
            }

            _fadeCanvas.alpha = targetAlpha;

            // 🔹 Якщо розтемнення — розблокуємо кліки
            if (Mathf.Approximately(targetAlpha, 0f))
            {
                _fadeCanvas.blocksRaycasts = false;
                _fadeCanvas.interactable = false;
            }
        }
    }
}