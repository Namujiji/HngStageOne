using HngStageOne.Application.Features.Profiles.Responses;

namespace HngStageOne.Application.Features.Agify.Responses;

public record AgifyResponse : IApiResponse
{
    public int Count { get; init; }
    public string? Name { get; init; }
    public int? Age { get; init; }
}
