using Services.Implementations;
using Services.Interfaces;
using Zenject;

namespace Installers
{
    public class ApplicationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ITabService>().To<TabService>().AsSingle();
        }
    }
}