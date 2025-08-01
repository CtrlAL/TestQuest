using Presenters;
using Services.Implementations;
using Services.Interfaces;
using UnityEngine;
using Views;
using Zenject;

namespace Installers
{
    public class BreedsAndFactsInstaller : MonoInstaller
    {
        [SerializeField] private BreedsView _view;
        [SerializeField] private BreedPopupView _popUpView;
        [SerializeField] private GameObject _breedItemPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(_view).AsSingle();
            Container.BindInstance(_popUpView).AsSingle();

            Container.BindFactory<BreedItemView, BreedItemView.BreedItemViewFactory>()
                     .FromComponentInNewPrefab(_breedItemPrefab)
                     .AsCached();

			Container.Bind<IDogApiService>().To<DogApiService>().AsSingle();

            Container.BindInterfacesAndSelfTo<BreedPopupPresenter>()
                     .AsSingle()
                     .NonLazy();

            Container.BindInterfacesAndSelfTo<BreedsPresenter>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}