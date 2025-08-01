using Models;
using Presenters;
using SO;
using UnityEngine;
using Views;
using Zenject;

namespace Installers
{
    public class ClickerInstaller : MonoInstaller
    {
        [SerializeField] private ClickerSettings _settings;
        [SerializeField] private ClickerView _view;
        [SerializeField] private GameObject _coinPrefub;
        [SerializeField] private Canvas _canvas;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<CurrencyPopup, CurrencyPopup.Pool>()
            .FromComponentInNewPrefab(_coinPrefub)
            .UnderTransform(_canvas.transform);

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