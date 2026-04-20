using System.Text.Json.Serialization;

namespace HngStageOne.Application.Features.Nationalize.Responses;

public class NationalizeResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("country")]
    public List<CountryProbability> Country { get; set; } = [];
}
public class CountryProbability
{
    [JsonPropertyName("country_id")]
    public string CountryId { get; set; } = null!;

    [JsonPropertyName("probability")]
    public double Probability { get; set; }
}