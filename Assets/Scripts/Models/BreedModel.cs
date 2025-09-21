using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models
{
    public class BreedAttributesModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "description")]
        public string Description;
        [JsonProperty(PropertyName = "life")]
        public Dictionary<string, int> Life;
        [JsonProperty(PropertyName = "male_weight")]
        public Dictionary<string, int> MaleWeight;
        [JsonProperty(PropertyName = "female_weight")]
        public Dictionary<string, int> FemaleWeight;
        [JsonProperty(PropertyName = "hypoallergenic")]
        public bool Hypoallergenic;
    }

    public class BreedNameModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name;
    }

    public class BreedModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id;
        [JsonProperty(PropertyName = "type")]
        public string Type;
        [JsonProperty(PropertyName = "attributes")]
        public BreedNameModel Attributes;
    }
}
