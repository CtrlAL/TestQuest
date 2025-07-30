using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class WeatherView : BaseView
    {
        public readonly Subject<Unit> OnRefreshRequested = new();

        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _forecastText;

        [SerializeField] private Button _refreshButton;

        private void Start()
        {
            _refreshButton.onClick.AddListener(() =>
            {
                OnRefreshRequested.OnNext(Unit.Default);
            });

            UpdateStatus("Готово");
        }

        public void SetActive(bool isActive)
        {
            base.SetActive(isActive);

            if (isActive)
            {
                UpdateStatus("Ожидание данных...");
            }
            else
            {
                UpdateStatus("Вкладка неактивна");
            }
        }

        public void UpdateStatus(string message)
        {
            _statusText.text = message;
        }

        public void UpdateForecast(string forecast)
        {
            _forecastText.text = forecast;
        }

        private void OnDestroy()
        {
            OnRefreshRequested?.Dispose();
        }
    }
}