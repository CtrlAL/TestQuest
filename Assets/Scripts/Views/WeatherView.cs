using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class WeatherView : MonoBehaviour
    {
        public readonly Subject<Unit> OnRefreshRequested = new();
        public bool IsActiveView { get; set; }

        [SerializeField] private TMP_Text _statusText;
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
            IsActiveView = isActive;
            gameObject.SetActive(isActive);

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

        private void OnDestroy()
        {
            OnRefreshRequested?.Dispose();
        }
    }
}