using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ResponseModels
{
	[Serializable]
	public class WeatherPeriodsResponse
	{
		[JsonProperty("properties")]
		public Properties Properties { get; set; }
	}

	[Serializable]
	public class Properties
	{
		[JsonProperty("periods")]
		public List<Period> Periods { get; set; }
	}

	[Serializable]
	public class Period
	{
		[JsonProperty("number")]
		public int Number { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("startTime")]
		public string StartTime { get; set; }

		[JsonProperty("endTime")]
		public string EndTime { get; set; }

		[JsonProperty("isDaytime")]
		public bool IsDaytime { get; set; }

		[JsonProperty("temperature")]
		public int Temperature { get; set; }

		[JsonProperty("temperatureUnit")]
		public string TemperatureUnit { get; set; }

		[JsonProperty("icon")]
		public string Icon { get; set; }

		[JsonProperty("shortForecast")]
		public string ShortForecast { get; set; }

		[JsonProperty("detailedForecast")]
		public string DetailedForecast { get; set; }
	}
}