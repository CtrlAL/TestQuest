using Models;
using Presenters;
using SO;
using UnityEngine;
using View;
using Zenject;

namespace Installers
{
    public class ClickerInstaller : MonoInstaller
    {
        [SerializeField] private ClickerSettings _settings;
        [SerializeField] private ClickerView _view;

        public override void InstallBindings()
        {
            Container.BindInstance(_settings).AsSingle();
            Container.BindInstance(_view).AsSingle();

            Container.Bind<CurrencyModel>().AsSingle();
            Container.Bind<EnergyModel>().AsSingle();

            Container.BindInterfacesAndSelfTo<ClickerPresenter>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}