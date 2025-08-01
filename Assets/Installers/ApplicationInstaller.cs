using Services.Implementations;
using Services.Interfaces;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ApplicationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ITabService>().To<TabService>().AsSingle();
            Container.Bind<MainView>().FromComponentInHierarchy().AsSingle();
        }
    }
}