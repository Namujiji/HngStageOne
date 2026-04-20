using Cortex.Mediator;
using Cortex.Mediator.Commands;
using HngStageOne.Application.Features.Profiles.Responses;
using HngStageOne.Application.Utilities;
using HngStageOne.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HngStageOne.Application.Features.Profiles.DeleteProfileById;

public sealed record DeleteProfileByIdCommand(Guid Id) : ICommand<Result<Unit>>;
public sealed record DeleteProfileByIdCommandHandler(
    ProfileDbContext DbContext,
    ILogger<DeleteProfileByIdCommandHandler> Logger) : ICommandHandler<DeleteProfileByIdCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteProfileByIdCommand query, CancellationToken cancellationToken)
    {
        var profile = await DbContext.Profiles.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (profile is null)
        {
            return await Result<Unit>.FailureAsync(new ErrorResponse("error", "Profile not found"));
        }

        DbContext.Profiles.Remove(profile);
        await DbContext.SaveChangesAsync(cancellationToken);

        return await Result<Unit>.SuccessAsync(Unit.Value);
    }
}