using SO;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace View
{
    [RequireComponent(typeof(AudioSource))]
    public class ClickerView : BaseView
    {
        [SerializeField] private Button _clickButton;
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private AudioSource _audioSource;

        [Inject] private ClickerSettings _settings;

        public readonly Subject<Unit> OnButtonClick = new();

        private void Start()
        {
            _clickButton.onClick.AddListener(() =>
            {
                OnButtonClick?.OnNext(Unit.Default);
                PlayFeedback();
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
            _currencyText.text = $"Coins: {amount}";
        }

        public void UpdateEnergyText(int amount)
        {
            _energyText.text = $"Energy: {amount}";
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

        private void OnDestroy()
        {
            OnButtonClick?.Dispose();
            OnActiveStateChanged?.Dispose();
        }
    }
}

