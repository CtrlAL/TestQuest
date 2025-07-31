namespace Services.Interfaces
{
    public interface ITabService
    {
        public Tab CurrentTub { get; }
        public void SwitchToTab(Tab tab);
    }

    public enum Tab
    {
        Clicker = 0,
        WeatherForecast,
        BreedsAndFacts
    }
}