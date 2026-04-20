using Cortex.Mediator.Queries;
using HngStageOne.Application.Abstractions.Services;
using HngStageOne.Application.Features.Genderize.Responses;
using HngStageOne.Application.Features.Profiles.Responses;
using HngStageOne.Application.Utilities;

namespace HngStageOne.Application.Features.Genderize.GetGenderByName;

public sealed record GetGenderByNameQuery(string Name) : IQuery<Result<GenderizeResponse>>;
public sealed record GetGenderByNameQueryHandler(
    IGenderizeClient GenderizeClient,
    ILogger<GetGenderByNameQueryHandler> Logger) : IQueryHandler<GetGenderByNameQuery, Result<GenderizeResponse>>
{
    public async Task<Result<GenderizeResponse>> Handle(GetGenderByNameQuery query, CancellationToken cancellationToken)
    {
        var genderizeResult = await GenderizeClient.GetGenderByPersonNameAsync(query.Name, cancellationToken);

        if (!genderizeResult.Succeeded)
        {
            return await Result<GenderizeResponse>.FailureAsync(genderizeResult.Error!);
        }

        if (genderizeResult.Data!.Count == 0 || string.IsNullOrWhiteSpace(genderizeResult.Data!.Gender))
        {
            return await Result<GenderizeResponse>.FailureAsync(new ErrorResponse("error", "No prediction available for the provided name"));
        }

        //return await Result<IProfileResponse>.SuccessAsync(new ProfileResponse("success", new Data
        //{
        //    Name = result.Name,
        //    Gender = result.Gender,
        //    IsConfident = result.Probability >= 0.7 && result.Count >= 100,
        //    Probability = result.Probability,
        //    ProcessedAt = DateTime.UtcNow,
        //    SampleSize = result.Count
        //}));

        return await Result<GenderizeResponse>.FailureAsync(new ErrorResponse("error", "No prediction available for the provided name"));
    }
}