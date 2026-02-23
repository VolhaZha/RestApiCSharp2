using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestApiCSharp.ConstantsTestingGeneral;

public class User
{
    [JsonProperty("age")]
    public int? Age { get; set; }

    [JsonProperty("name")] 
    public string Name { get; set; }

    [JsonProperty("sex")]
    [JsonConverter(typeof(StringEnumConverter))]
    public Sex Sex { get; set; }

    [JsonProperty("zipCode")] 
    public string ZipCode { get; set; }
}
