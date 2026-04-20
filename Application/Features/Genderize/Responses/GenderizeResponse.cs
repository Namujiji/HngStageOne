using HngStageOne.Application.Features.Profiles.Responses;

namespace HngStageOne.Application.Features.Genderize.Responses;

public record GenderizeResponse : IApiResponse
{
    public int Count { get; init; }
    public string? Name { get; init; }
    public string? Gender { get; init; }
    public double Probability { get; init; }
}
