using Services.Implementations;

namespace Services.Interfaces
{
    public interface ITabService
    {
        public Tab CurrentTub { get; }
        public void SwitchToTab(Tab tab);

        public void MoveNext();

        public void MoveBack();
	}

        public enum Tab
        {
            Clicker = 0,
            WeatherForecast,
            BreedsAndFacts
        }
}