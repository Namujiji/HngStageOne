using Cortex.Mediator.Queries;
using HngStageOne.Application.Features.Profiles.Responses;
using HngStageOne.Application.Utilities;
using HngStageOne.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HngStageOne.Application.Features.Profiles.GetProfileById;

public sealed record GetProfileByIdQuery(Guid Id) : IQuery<Result<IProfileResponse>>;
public sealed record GetProfileByIdQueryHandler(
    ProfileDbContext DbContext,
    ILogger<GetProfileByIdQueryHandler> Logger) : IQueryHandler<GetProfileByIdQuery, Result<IProfileResponse>>
{
    public async Task<Result<IProfileResponse>> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken)
    {
        var profile = await DbContext.Profiles.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (profile is null)
        {
            return await Result<IProfileResponse>.FailureAsync(new ErrorResponse("error", "Profile not found"));
        }

        return await Result<IProfileResponse>.SuccessAsync(new SuccessResponse("success", ProfileResponse.FromProfile(profile)));
    }
}