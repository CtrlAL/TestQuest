using Models;
using SO;
using UnityEngine;
using Views;
using Zenject;

namespace Installers
{
    public class WeatherForecastInstaller : MonoInstaller
    {
        [SerializeField] private WeatherSettingsSO _settings;
        [SerializeField] private WeatherView _view;
        public override void InstallBindings()
        {
            Container.BindInstance(_settings).AsSingle();
            Container.BindInstance(_view).AsSingle();

            Container.Bind<WeatherModel>().AsSingle();

            Container.BindInterfacesAndSelfTo<WeatherPresenter>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}