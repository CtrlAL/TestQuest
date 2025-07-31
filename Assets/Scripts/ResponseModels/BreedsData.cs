using Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ResponseModels
{
	public class BreedsResponse
	{
		[JsonProperty("data")]
		public List<BreedModel> Data;
	}

	public class BreedDetailResponse
	{
		[JsonProperty("data")]
		public BreedDetailData Data;
	}

	public class BreedDetailData
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("attributes")]
		public Attributes Attributes;
	}

	public class Attributes
	{
		[JsonProperty("name")]
		public string Name;

		[JsonProperty("fact")]
		public string Fact;

		[JsonProperty("iamge")]
		public string Image;
	}
}