using SO;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Models
{
    public class EnergyModel : IInitializable, IDisposable
    {
        public readonly ReactiveProperty<int> Count = new();

        private readonly ClickerSettings _settings;

        public EnergyModel(ClickerSettings settings)
        {
            _settings = settings;
        }

        public void Initialize()
        {
            Count.Value = _settings.MaxEnergy;
        }

        public void Spend(int amount)
        {
            Count.Value = Mathf.Max(0, Count.Value - amount);
        }

        public void Regenerate(int amount)
        {
            Count.Value = Mathf.Min(_settings.MaxEnergy, Count.Value + amount);
        }

        public bool HasEnough(int cost)
        {
            return Count.Value >= cost;
        }

        public void Dispose()
        {
            Count?.Dispose();
        }
    }
}

