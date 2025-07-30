using Cysharp.Threading.Tasks;
using Services.Interfaces;
using SO;
using System.Threading;
using UnityEngine.Networking;

namespace Services.Implementations
{
    public class WeatherService : IWeatherService
    {
        private readonly WeatherSettingsSO _settings;

        public WeatherService(WeatherSettingsSO settings)
        {
            _settings = settings;
        }

        public async UniTask<string> FetchForecastAsync(CancellationToken ct)
        {
            using (var request = UnityWebRequest.Get(_settings.WeatherUrl))
            {
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    ct.ThrowIfCancellationRequested();
                    await UniTask.Yield();
                }

                ct.ThrowIfCancellationRequested();

                if (request.result == UnityWebRequest.Result.ConnectionError || 
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    throw new System.Exception($"Request failed: {request.error} (HTTP {request.responseCode})");
                }

                return request.downloadHandler.text;
            }
        }
    }
}