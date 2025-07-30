using UniRx;
using UnityEngine;

namespace View
{
    public class BaseView : MonoBehaviour
    {
        public bool IsActiveView { get; set; }

        public readonly Subject<bool> OnActiveStateChanged = new();

        public void SetActive(bool isActive)
        {
            if (IsActiveView == isActive) return;

            IsActiveView = isActive;
            OnActiveStateChanged.OnNext(isActive);

            gameObject.SetActive(isActive);
        }
    }
}


