using System;

namespace Models
{
    public class WeatherModel
    {
        public string Forecast { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool HasData => !string.IsNullOrEmpty(Forecast);
    }
}

