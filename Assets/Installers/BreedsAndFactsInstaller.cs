using Presenters;
using Services.Implementations;
using UnityEngine;
using Views;
using Zenject;

namespace Installers
{
    public class BreedsAndFactsInstaller : MonoInstaller
    {
        [SerializeField] private BreedsView _view;
        [SerializeField] private GameObject _breedItemPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(_view).AsSingle();

            Container.BindFactory<BreedItemView, BreedItemView.BreedItemViewFactory>()
                     .FromComponentInNewPrefab(_breedItemPrefab)
                     .AsCached();

            Container.Bind<DogApiService>().AsSingle();
            Container.BindInterfacesAndSelfTo<BreedsPresenter>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}