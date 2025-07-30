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
            Container.Bind<IWeatherService>().To<WeatherService>().AsSingle();
            Container.Bind<IDogApiService>().To<DogApiService>().AsSingle();
        }
    }
}