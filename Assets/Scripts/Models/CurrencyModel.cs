using SO;
using System;
using UniRx;
using Zenject;

namespace Models
{
    public class CurrencyModel : IInitializable, IDisposable
    {
        public readonly ReactiveProperty<int> Amount = new();

        private readonly ClickerSettings _settings;

        public CurrencyModel(ClickerSettings settings)
        {
            _settings = settings;
        }

        public void Initialize()
        {
            Amount.Value = _settings.StartingCurrency;
        }

        public void Add(int amount)
        {
            Amount.Value += amount;
        }

        public bool CanAfford(int cost)
        {
            return Amount.Value >= cost;
        }

        public void Dispose()
        {
            Amount?.Dispose();
        }
    }
}

