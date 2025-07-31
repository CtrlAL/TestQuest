using UniRx;
using UnityEngine;

namespace Views
{
    public class BaseView : MonoBehaviour
    {
        public bool IsActiveView 
        { 
            get { return gameObject.activeSelf; } 
            set { gameObject.SetActive(value); } 
        }

        public readonly Subject<bool> OnActiveStateChanged = new();

        public virtual void SetActive(bool isActive)
        {
            if (IsActiveView == isActive) return;

			IsActiveView = isActive;
			OnActiveStateChanged.OnNext(isActive);
		}
    }
}


