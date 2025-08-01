using Models;
using TMPro;
using UnityEngine;

namespace Views
{
    public class BreedPopupView : BaseView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _fact;

        public void Show()
        {
            SetActive(true);
        }

        public void Hide()
        {
            SetActive(false);
        }

        public void SetContent(BreedDetailsModel detail)
        {
            _title.text = detail.Name;
            _fact.text = detail.Fact;
        }
    }
}