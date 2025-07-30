using Cysharp.Threading.Tasks;
using Models;
using Services.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Services.Implementations
{
    public class DogApiService : IDogApiService
    {
        private const string _baseUrl = "https://dogapi.dog/api/v2";

        public async UniTask<List<BreedModel>> GetBreedsAsync(System.Threading.CancellationToken ct)
        {
            var request = UnityWebRequest.Get($"{_baseUrl}/breeds?page[size]=10");
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                if (ct.IsCancellationRequested)
                {
                    request.Abort();
                    throw new System.OperationCanceledException(ct);
                }
                await UniTask.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
                throw new System.Exception($"Ошибка: {request.error}");

            var response = JsonUtility.FromJson<BreedsResponse>(request.downloadHandler.text);
            return response.Data;
        }

        public async UniTask<BreedDetailsModel> GetBreedByIdAsync(int id, System.Threading.CancellationToken ct)
        {
            var request = UnityWebRequest.Get($"{_baseUrl}/breeds/{id}");
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                if (ct.IsCancellationRequested)
                {
                    request.Abort();
                    throw new System.OperationCanceledException(ct);
                }
                await UniTask.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
                throw new System.Exception($"Ошибка: {request.error}");

            var response = JsonUtility.FromJson<BreedDetailResponse>(request.downloadHandler.text);
            var data = response.Data;

            return new BreedDetailsModel
            {
                Id = data.Id,
                Name = data.Attributes.Name,
                Fact = data.Attributes.Fact ?? "Нет факта",
                ImageUrl = data.Attributes.Image ?? ""
            };
        }

        [System.Serializable]
        private class BreedsResponse
        {
            public List<BreedModel> Data;
        }

        [System.Serializable]
        private class BreedDetailResponse
        {
            public BreedDetailData Data;
        }

        [System.Serializable]
        private class BreedDetailData
        {
            public int Id;
            public Attributes Attributes;
        }

        [System.Serializable]
        private class Attributes
        {
            public string Name;
            public string Fact;
            public string Image;
        }
    }
}