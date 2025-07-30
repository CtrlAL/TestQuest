namespace Services.Interfaces
{
    public interface ITabService
    {
        public void SwitchToTab(Tab tab);
    }

    public enum Tab
    {
        Clicker,
        WeatherForecast,
        BreedsAndFacts
    }
}