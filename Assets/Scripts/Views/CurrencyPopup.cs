using UnityEngine;
using Zenject;

namespace Views
{
    public class CurrencyPopup : MonoBehaviour
    {
        public class Pool : MemoryPool<CurrencyPopup>
        {
            protected override void OnDespawned(CurrencyPopup item)
            {
                item.gameObject.SetActive(false);
            }

            protected override void OnSpawned(CurrencyPopup item)
            {
                item.gameObject.SetActive(true);
            }
        }
    }
}