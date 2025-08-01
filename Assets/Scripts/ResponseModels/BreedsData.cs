using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ResponseModels
{
	public class BreedsResponse
	{
		[JsonProperty("data")]
		public List<BreedDetailData> Data { get; set; }
	}

	public class BreedDetailResponse
	{
		[JsonProperty("data")]
		public BreedDetailData Data { get; set; }
    }

	public class BreedDetailData
	{
		[JsonProperty("id")]
		public Guid Id;
		[JsonProperty("attributes")]
		public Attributes Attributes { get; set; }
    }

	public class Attributes
	{
		[JsonProperty("name")]
		public string Name { get; set; }

        [JsonProperty("description")]
		public string Fact { get; set; }
    }
}