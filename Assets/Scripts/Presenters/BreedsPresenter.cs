using Cysharp.Threading.Tasks;
using Services.Interfaces;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using Views;
using Zenject;

namespace Presenters
{
    public class BreedsPresenter : IInitializable, IDisposable
    {
        [Inject] private IDogApiService _apiService;
        [Inject] private BreedsView _view;
        [Inject] private BreedPopupView _popUpView;

        private readonly CompositeDisposable _disposables = new();

        private CancellationTokenSource _breedsCts;
        private CancellationTokenSource _detailsCts;

        public void Initialize()
        {
            _view.OnActiveStateChanged
                .Subscribe(OnActiveChanged)
                .AddTo(_disposables);

            _view.OnBreedSelected
                .Subscribe(OnBreedSelected)
                .AddTo(_disposables);
        }

        private void OnActiveChanged(bool isActive)
        {
            if (isActive)
            {
                LoadBreeds();
            }
            else
            {
                CancelAllRequests();
            }
        }

        private async void LoadBreeds()
        {
            _breedsCts?.Cancel();
            _breedsCts = new CancellationTokenSource();

            _view.ShowLoader();

            try
            {
                var breeds = await _apiService.GetBreedsAsync(_breedsCts.Token);

                if (!_breedsCts.IsCancellationRequested)
                {
                    _view.HideLoader();
                    _view.PopulateBreeds(breeds);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("�������� ������ ����� ��������.");
            }
            catch (Exception e)
            {
                if (!(_breedsCts?.IsCancellationRequested ?? true))
                {
                    Debug.LogError("������ �������� �����: " + e.Message);
                    _view.HideLoader();
                }
            }
        }

        private async void OnBreedSelected((Guid breedId, BreedItemView itemView) args)
        {
            _detailsCts?.Cancel();
            _detailsCts = new CancellationTokenSource();
            args.itemView.ShowLoader();

            Debug.Log($"������ ������ � ������: {args.breedId}");

            try
            {
                var details = await _apiService.GetBreedByIdAsync(args.breedId, _detailsCts.Token);

                if (!_detailsCts.IsCancellationRequested)
                {
                    Debug.Log($"�������� ������ � ������ {args.breedId}: {details.Fact}");

                    args.itemView.HideLoader();
                    _popUpView.SetContent(details);
                    _popUpView.Show();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"������ � ������ {args.breedId} �������.");
            }
            catch (Exception e)
            {
                if (!(_detailsCts?.IsCancellationRequested ?? true))
                {
                    Debug.LogError($"������ �������� ������ {args.breedId}: " + e.Message);
                }
            }
        }

        private void CancelAllRequests()
        {
            _breedsCts?.Cancel();
            _detailsCts?.Cancel();
            _view.HideLoader();
        }

        public void Dispose()
        {
            CancelAllRequests();

            _breedsCts?.Dispose();
            _detailsCts?.Dispose();
            _disposables?.Dispose();
        }
    }
}