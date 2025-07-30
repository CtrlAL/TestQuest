using Cysharp.Threading.Tasks;
using Services.Implementations;
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
        [Inject] private DogApiService _apiService;
        [Inject] private BreedsView _view;

        private readonly CompositeDisposable _disposables = new();
        private CancellationTokenSource _cts;

        public void Initialize()
        {
            _view.OnActiveStateChanged
                .Subscribe(_ => LoadBreeds())
                .AddTo(_disposables);

            _view.OnBreedSelected
                .Subscribe(id => LoadBreedDetails(id))
                .AddTo(_disposables);
        }

        private async void LoadBreeds()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _view.ShowLoader();

            try
            {
                var breeds = await _apiService.GetBreedsAsync(_cts.Token);
                if (!_cts.IsCancellationRequested)
                {
                    _view.HideLoader();
                    _view.PopulateBreeds(breeds);
                }
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("Загрузка пород отменена");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка загрузки пород: " + e.Message);
                _view.HideLoader();
            }
        }

        private async void LoadBreedDetails(int id)
        {
            Debug.Log($"Запрос данных о породе: {id}");
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _disposables?.Dispose();
        }
    }
}