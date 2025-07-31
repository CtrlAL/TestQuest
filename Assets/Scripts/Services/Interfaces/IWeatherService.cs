using Cysharp.Threading.Tasks;
using Models;
using System.Threading;

namespace Services.Interfaces
{
    public interface IWeatherService
    {
        public UniTask<WeatherRequestData> FetchForecastAsync(CancellationToken ct);
    }
}