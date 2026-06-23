using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class ScreenTransitionService : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _fadeCanvas;
        [SerializeField] private float _fadeDuration = 1f;
        private readonly float _startAlpha = 0f;

        private void Start()
        {
            _fadeCanvas.alpha = _startAlpha;
        }

        public async UniTask FadeAsync(float targetAlpha)
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