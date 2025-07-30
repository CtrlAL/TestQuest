using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "ClickerSettings", menuName = "Clicker/Game Settings", order = 1)]
    public class ClickerSettings : ScriptableObject
    {
        [Header("Currency Settings")]
        [SerializeField] private int _startingCurrency = 0;
        [SerializeField] private int _clickAmount = 1;
        [SerializeField] private int _autoCollectAmount = 1;

        [Header("Autoclick Settings")]
        [SerializeField] private float _autoCollectInterval = 3f;
        [SerializeField] private bool _autoClickEnabled = true;

        [Header("Energy Settings")]
        [SerializeField] private int _maxEnergy = 5;
        [SerializeField] private int _energyCostPerClick = 1;
        [SerializeField] private float _energyRegenInterval = 5f;
        [SerializeField] private int _energyRegenCount = 5;

        [Header("Feedback Settings")]
        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private ParticleSystem _clickVFX;
        [SerializeField] private float _feedbackDuration = 0.5f;

        public int StartingCurrency => _startingCurrency;
        public int ClickAmount => _clickAmount;
        public int AutoCollectAmount => _autoCollectAmount;
        public float AutoCollectInterval => _autoCollectInterval;
        public bool AutoClickEnabled => _autoClickEnabled;
        public int MaxEnergy => _maxEnergy;
        public int EnergyCostPerClick => _energyCostPerClick;
        public float EnergyRegenInterval => _energyRegenInterval;
        public int EnergyRegenCount => _energyRegenCount;
        public AudioClip ClickSound => _clickSound;
        public ParticleSystem ClickVFX => _clickVFX;
        public float FeedbackDuration => _feedbackDuration;
    }
}

