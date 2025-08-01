using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
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
                    throw new OperationCanceledException(ct);
                }
                await UniTask.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception($"Ошибка: {request.error}");

            var response = JsonConvert.DeserializeObject<BreedsResponse>(request.downloadHandler.text);

            List<BreedModel> list = new();

            foreach (var bred in response.Data)
            {
                list.Add(new BreedModel
                {
                    Id = bred.Id,
                    Name = bred.Attributes.Name,
                });
            }

            return list;
        }

        public async UniTask<BreedDetailsModel> GetBreedByIdAsync(Guid id, System.Threading.CancellationToken ct)
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

            var response = JsonConvert.DeserializeObject<BreedDetailResponse>(request.downloadHandler.text);
            var data = response.Data;

            return new BreedDetailsModel
            {
                Id = data.Id,
                Name = data.Attributes.Name,
                Fact = data.Attributes.Fact ?? "Нет факта",
            };
        }
    }
}