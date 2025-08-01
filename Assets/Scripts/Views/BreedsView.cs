using Models;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Views
{
    public class BreedsView : BaseView
    {
        [SerializeField] private GameObject _loader;
        [SerializeField] private Transform _listContainer;
        [SerializeField] private GameObject _breedItemPrefab;

        public readonly Subject<Guid> OnBreedSelected = new();
        private readonly List<GameObject> _spawnedItems = new();

        public void ShowLoader()
        {
            _loader.SetActive(true);
            HideList();
        }

        public void HideLoader()
        {
            _loader.SetActive(false);
        }

        public void PopulateBreeds(List<BreedModel> breeds)
        {
            HideList();

            for (int i = 0; i < breeds.Count; i++)
            {
                var itemGO = Instantiate(_breedItemPrefab, _listContainer);
                var itemView = itemGO.GetComponent<BreedItemView>();
                itemView.SetData(i + 1, breeds[i].Name);
                itemView.OnClicked.Subscribe(_ => OnBreedSelected.OnNext(breeds[i].Id)).AddTo(itemGO);
                _spawnedItems.Add(itemGO);
            }
        }

        private void HideList()
        {
            foreach (var go in _spawnedItems)
                Destroy(go);
            _spawnedItems.Clear();
        }

        private void OnDestroy()
        {
            OnActiveStateChanged?.Dispose();
            OnBreedSelected?.Dispose();
        }
    }
}