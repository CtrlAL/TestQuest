using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "WeatherSettings", menuName = "Weather/Settings", order = 2)]
    public class WeatherSettingsSO : ScriptableObject
    {
        [Header("API")]
        public string WeatherUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        [Header("Update")]
        public float UpdateInterval = 5f;

        [Header("Feedback")]
        public AudioClip loadingSound;
        public ParticleSystem successVFX;
    }
}
