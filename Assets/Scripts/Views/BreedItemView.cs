using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Views
{
    public class BreedItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _button;

        public readonly Subject<Unit> OnClicked = new();

        public void SetData(int number, string name)
        {
            _numberText.text = $"{number} -";
            _nameText.text = name;
            _button.onClick.AddListener(() => OnClicked.OnNext(Unit.Default));
        }

        private void OnDestroy()
        {
            OnClicked?.Dispose();
        }

        public class BreedItemViewFactory : PlaceholderFactory<BreedItemView>
        {
        }
    }
}
