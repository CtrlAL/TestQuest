using Cysharp.Threading.Tasks;
using System.Threading;

namespace Services.Interfaces
{
    public interface IWeatherService
    {
        public UniTask<string> FetchForecastAsync(CancellationToken ct);
    }
}