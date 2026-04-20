using HngStageOne.Domain.Models;
using System.Text.Json.Serialization;

namespace HngStageOne.Application.Features.Profiles.Responses;

public sealed record ProfileResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("gender")]
    public string Gender { get; init; } = null!;

    [JsonPropertyName("gender_probability")]
    public double GenderProbability { get; init; }

    [JsonPropertyName("sample_size")]
    public int SampleSize { get; init; }

    [JsonPropertyName("age")]
    public int Age { get; init; }

    [JsonPropertyName("age_group")]
    public string AgeGroup { get; init; } = null!;

    [JsonPropertyName("country_id")]
    public string CountryId { get; init; } = null!;

    [JsonPropertyName("country_probability")]
    public double CountryProbability { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    internal static ProfileResponse FromProfile(Profile profile)
    {
        return new ProfileResponse
        {
            Id = profile.Id,
            Age = profile.Age,
            AgeGroup = profile.AgeGroup,
            CountryId = profile.CountryId,
            CountryProbability = profile.CountryProbability,
            CreatedAt = profile.CreatedAt,
            Gender = profile.Gender,
            GenderProbability = profile.GenderProbability,
            Name = profile.Name,
            SampleSize = profile.SampleSize
        };
    }
}