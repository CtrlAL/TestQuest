using SO;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;

namespace Views
{
    [RequireComponent(typeof(AudioSource))]
    public class ClickerView : BaseView
    {
        [SerializeField] private Image _coinImageUI;
        [SerializeField] private Button _clickButton;
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Canvas _canvas;

        [Inject] private CurrencyPopup.Pool _flyItemPool;
        [Inject] private ClickerSettings _settings;

        public readonly Subject<Unit> OnButtonClick = new();

        private void Start()
        {
            _clickButton.onClick.AddListener(() =>
            {
                OnButtonClick?.OnNext(Unit.Default);
                PlayFeedback();
                FlyCoin();
            });

            _audioSource = GetComponent<AudioSource>();
        }

        public void Initialize()
        {
            UpdateCurrencyText(_settings.StartingCurrency);
            UpdateEnergyText(_settings.MaxEnergy);

            IsActiveView = true;
        }

        public void UpdateCurrencyText(int amount)
        {
            _currencyText.text = $"{amount}";
        }

        public void UpdateEnergyText(int amount)
        {
            _energyText.text = $"{amount}";
        }

        public void PlayFeedback()
        {
            if (_settings.ClickVFX != null)
            {
                var vfx = Instantiate(_settings.ClickVFX, _clickButton.transform.position, Quaternion.identity);
                Destroy(vfx.gameObject, vfx.main.duration);
            }

            if (_settings.ClickSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_settings.ClickSound);
            }
        }

        private void FlyCoin()
        {
            Vector2 from = GetCanvasCenter(_clickButton.GetComponent<RectTransform>());
            Vector2 to = GetCanvasCenter(_coinImageUI.GetComponent<RectTransform>());

            CurrencyPopup flyItem = _flyItemPool.Spawn();
            flyItem.gameObject.SetActive(true);
            RectTransform rect = flyItem.GetComponent<RectTransform>();

            rect.anchoredPosition = from;
            rect.localScale = Vector3.one;

            _ = AnimateFly(flyItem, rect, from, to);
        }

        private async UniTask AnimateFly(CurrencyPopup flyItem, RectTransform rect, Vector2 from, Vector2 to, float duration = 1f)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                t = Mathf.SmoothStep(0, 1, t);

                rect.anchoredPosition = Vector2.Lerp(from, to, t);
                rect.localScale = Vector3.one * (1f - t * 0.3f);

                if (rect.TryGetComponent<CanvasRenderer>(out var renderer))
                {
                    renderer.SetAlpha(1f - t);
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            flyItem.gameObject.SetActive(false);
            _flyItemPool.Despawn(flyItem);
        }

        private Vector2 GetCanvasCenter(RectTransform rt)
        {
            Vector3 worldPoint = rt.TransformPoint(rt.rect.center);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.GetComponent<RectTransform>(),
                worldPoint,
                null,
                out Vector2 localPoint
            );
            return localPoint;
        }

        private void OnDestroy()
        {
            OnButtonClick?.Dispose();
            OnActiveStateChanged?.Dispose();
        }
    }
}

