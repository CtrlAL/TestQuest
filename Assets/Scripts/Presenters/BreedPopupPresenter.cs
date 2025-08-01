using System.Threading;
using System;
using UnityEngine;
using Views;
using Zenject;
using Services.Interfaces;

namespace Presenters
{
    public class BreedPopupPresenter : MonoBehaviour
    {
        [SerializeField] private BreedPopupView _view;
        [Inject] private IDogApiService _breedService;

        private CancellationTokenSource _currentRequestCts;
        private CancellationTokenSource _pageCts;

        private void Awake()
        {
            _pageCts = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            _pageCts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            _pageCts.Cancel();
            _currentRequestCts?.Cancel();
            _view.Hide();
        }

        private void OnDestroy()
        {
            _pageCts?.Cancel();
            _pageCts?.Dispose();
            _currentRequestCts?.Cancel();
            _currentRequestCts?.Dispose();
        }

        public void OnBreedSelected(Guid breedId)
        {
            _currentRequestCts?.Cancel();

            //_view.SetLoading(true);
            _view.Show();

            LoadBreedDetails(breedId);
        }

        private async void LoadBreedDetails(Guid breedId)
        {
            _currentRequestCts = new CancellationTokenSource();

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                _currentRequestCts.Token,
            _pageCts.Token
            );

            try
            {
                var detail = await _breedService.GetBreedByIdAsync(breedId, linkedCts.Token);

                if (!linkedCts.Token.IsCancellationRequested)
                {
                    _view.SetContent(detail);
                    //_view.SetLoading(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                if (_pageCts.IsCancellationRequested || _currentRequestCts.IsCancellationRequested)
                    return;

                Debug.LogError($"Ошибка загрузки породы {breedId}: {e.Message}");
                //_view.SetLoading(false);
            }
        }
    }
}