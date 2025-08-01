﻿using Cysharp.Threading.Tasks;
using Models;
using SO;
using System.Threading;
using System;
using UniRx;
using Views;
using Zenject;
using Services.Interfaces;

public class WeatherPresenter : IInitializable, IDisposable
{
    [Inject] private IWeatherService _weatherService;
    [Inject] private WeatherView _view;
    [Inject] private WeatherSettingsSO _settings;

    private readonly CompositeDisposable _disposables = new();
    private CancellationTokenSource _cts;
    private WeatherModel _model = new();

    public void Initialize()
    {
        _view.OnRefreshRequested
            .Subscribe(async _ => await RefreshWeather())
            .AddTo(_disposables);

        _view.OnActiveStateChanged
            .Subscribe(OnActiveChanged)
            .AddTo(_disposables);

        if (_view.IsActiveView)
        {
            StartAutoUpdate();
        }
    }

    private void OnActiveChanged(bool isActive)
    {
        if (isActive)
        {
            StartAutoUpdate();
        }
        else
        {
            StopAutoUpdate();
        }
    }

    private void StartAutoUpdate()
    {
        StopAutoUpdate();
        _cts = new CancellationTokenSource();
        UpdateLoop(_cts.Token).Forget();
    }

    private void StopAutoUpdate()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async UniTaskVoid UpdateLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_settings.UpdateInterval),
                                    cancellationToken: ct);

                if (!_view.IsActiveView || ct.IsCancellationRequested) break;

                await RefreshWeather();
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!ct.IsCancellationRequested)
                {
                    _view.UpdateStatus($"Ошибка: {e.Message}");
                }
            }
        }
    }

    private async UniTask RefreshWeather()
    {
        try
        {
            _view.UpdateStatus("Загрузка...");

            var weatherRequestData = await _weatherService.FetchForecastAsync(_cts?.Token ?? default);

            _model.Forecast = weatherRequestData.Forecast;
            _model.LastUpdated = DateTime.Now;

            _view.UpdateStatus($"Обновлено: {_model.LastUpdated:HH:mm:ss}");
            _view.UpdateForecast(weatherRequestData.Forecast);
            _view.UpdateIcon(weatherRequestData.Icon);
        }
        catch (Exception e)
        {
            if (!(_cts?.IsCancellationRequested ?? true))
            {
                _view.UpdateStatus($"Ошибка: {e.Message}");
            }
        }
    }

    public void Dispose()
    {
        StopAutoUpdate();
        _disposables?.Dispose();
    }
}