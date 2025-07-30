using Models;
using SO;
using System;
using UniRx;
using UnityEngine;
using View;
using Zenject;

namespace Presenters
{
    public class ClickerPresenter : IInitializable, ITickable, IDisposable
    {
        [Inject] private CurrencyModel _currency;
        [Inject] private EnergyModel _energy;
        [Inject] private ClickerView _view;
        [Inject] private ClickerSettings _settings;

        private readonly CompositeDisposable _disposables = new();

        public void Initialize()
        {
            
            _currency.Amount
                .Subscribe(_view.UpdateCurrencyText)
                .AddTo(_disposables);

            _energy.Count
                .Subscribe(_view.UpdateEnergyText)
                .AddTo(_disposables);

            _view.OnButtonClick
                .Where(_ => _energy.HasEnough(_settings.EnergyCostPerClick))
                .Subscribe(_ =>
                {
                    _currency.Add(_settings.ClickAmount);
                    _energy.Spend(_settings.EnergyCostPerClick);
                })
                .AddTo(_disposables);

            Observable.Interval(TimeSpan.FromSeconds(_settings.AutoCollectInterval))
                .Where(_ => _view.IsActiveView && _settings.AutoClickEnabled)
                .Do(x => Debug.Log($"[AutoClick] Tick {x}"))
                .Subscribe(_ =>
                {
                    _currency.Add(_settings.AutoCollectAmount);
                })
                .AddTo(_disposables);

            Observable.Interval(TimeSpan.FromSeconds(_settings.EnergyRegenInterval))
                .Do(x => Debug.Log($"[Regen] Tick {x}"))
                .Where(_ => _view.IsActiveView && _energy.Count.Value < _settings.MaxEnergy)
                .Subscribe(_ =>
                {
                    _energy.Regenerate(_settings.EnergyRegenCount);
                })
                .AddTo(_disposables);

            _currency.Initialize();
            _energy.Initialize();
            _view.Initialize();
        }

        public void Tick()
        {
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}