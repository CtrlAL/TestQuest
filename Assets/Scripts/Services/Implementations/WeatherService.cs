using Cysharp.Threading.Tasks;
using Services.Interfaces;
using SO;
using System.Net.Http;
using System.Threading;

namespace Services.Implementations
{
    public class WeatherService : IWeatherService
    {
        private readonly WeatherSettingsSO _settings;
        private readonly HttpClient _httpClient;

        public WeatherService(WeatherSettingsSO settings)
        {
            _settings = settings;
            _httpClient = new HttpClient();
        }

        public async UniTask<string> FetchForecastAsync(CancellationToken ct)
        {
            return await _httpClient.GetStringAsync(_settings.WeatherUrl);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}