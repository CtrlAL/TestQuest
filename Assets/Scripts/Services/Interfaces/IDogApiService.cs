using Cysharp.Threading.Tasks;
using Models;
using System;
using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface IDogApiService
    {
        public UniTask<List<BreedModel>> GetBreedsAsync(System.Threading.CancellationToken ct);

        public UniTask<BreedDetailsModel> GetBreedByIdAsync(Guid id, System.Threading.CancellationToken ct);
    }
}