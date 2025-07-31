using Services.Interfaces;
using System;
using Views;
using Zenject;

namespace Services.Implementations
{
    public class TabService : IInitializable, ITickable, ITabService
    {
        private readonly ClickerView _clickerView;
        private readonly WeatherView _watherForecast;
        private readonly BreedsView _breedsAndFacts;

        private Tab _currentTab;
		public static int Count => Enum.GetValues(typeof(Tab)).Length;

		public Tab CurrentTub => _currentTab;

		public TabService(ClickerView clickerView, WeatherView watherForecast, BreedsView breedsAndFacts)
        {
            _clickerView = clickerView;
            _watherForecast = watherForecast;
            _breedsAndFacts = breedsAndFacts;
        }

        public void Initialize()
        {
        }

        public void Tick()
        {
        }

        public void SwitchToTab(Tab tab)
        {
            if (_currentTab == tab) return;

            DeactivateCurrentTab();

            _currentTab = tab;
            ActivateTab(tab);
        }

        private void DeactivateCurrentTab()
        {
            switch (_currentTab)
            {
                case Tab.Clicker:
                    _clickerView.SetActive(false);
                    break;
                case Tab.WeatherForecast:
                    _watherForecast.SetActive(false);
                    break;
                case Tab.BreedsAndFacts:
                    _breedsAndFacts.SetActive(false);
                    break;
            }
        }

        private void ActivateTab(Tab tab)
        {
            switch (tab)
            {
                case Tab.Clicker:
                    _clickerView.SetActive(true);
                    break;
                case Tab.WeatherForecast:
                    _watherForecast.SetActive(true);
                    break;
                case Tab.BreedsAndFacts:
                    _breedsAndFacts.SetActive(true);
                    break;
            }
        }

        public Tab GetCurrentTab() => _currentTab;

		public void MoveNext()
		{
			var next = ((int)_currentTab + 1) % Count;
			SwitchToTab((Tab)next);
		}

		public void MoveBack()
		{
			var prev = ((int)_currentTab - 1 + Count) % Count;
			SwitchToTab((Tab)prev);
		}
	}
}

