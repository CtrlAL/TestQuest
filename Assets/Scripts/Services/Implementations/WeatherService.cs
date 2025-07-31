using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using ResponseModels;
using Services.Interfaces;
using SO;
using System.Linq;
using System.Threading;
using UnityEngine;
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

        public async UniTask<WeatherRequestData> FetchForecastAsync(CancellationToken ct)
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

                var responseText = request.downloadHandler.text;
                var periods = JsonConvert.DeserializeObject<WeatherPeriodsResponse>(responseText);
                var resultText = periods.Properties.Periods.Where(x => x.Name == "Today").FirstOrDefault();

                var text = resultText != null ? $"{resultText.Name} - {resultText.Temperature} {resultText.TemperatureUnit}" : "Прогноз на сегодня не найден";
                var icon = await LoadIconAsync(resultText.Icon);

                return new WeatherRequestData
                {
                    Icon = icon,
                    Forecast = text,
                };
            }
        }

		public async UniTask<Sprite> LoadIconAsync(string url)
		{
			using (var request = UnityWebRequestTexture.GetTexture(url))
			{
				await request.SendWebRequest();

				if (request.result == UnityWebRequest.Result.Success)
				{
					var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
					return Sprite.Create(texture,
						new Rect(0, 0, texture.width, texture.height),
						Vector2.one * 0.5f);
				}
				else
				{
					Debug.LogError(request.error);
					return null;
				}
			}
		}
	}
}