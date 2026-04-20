using Cortex.Mediator.Queries;
using FluentValidation;
using HngStageOne.Application.Features.Profiles.Responses;
using HngStageOne.Application.Utilities;
using HngStageOne.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HngStageOne.Application.Features.Profiles.GetAllProfiles;

public record GetAllProfilesQuery(string? Gender, string? CountryId, string? AgeGroup) : IQuery<Result<IProfileResponse>>;

public class GetAllProfilesQueryValidator : AbstractValidator<GetAllProfilesQuery>
{
    public GetAllProfilesQueryValidator()
    {
        When(p => !string.IsNullOrWhiteSpace(p.Gender), () =>
        {
            RuleFor(n => n.Gender)
                .Must(gender => gender!.All(char.IsLetter)).WithMessage("{PropertyName} must contain only letters.")
                .Must(gender => gender!.Equals("Male", StringComparison.InvariantCultureIgnoreCase) || gender!.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("{PropertyName} must be either 'Male' or 'Female'.");
        });

        When(p => !string.IsNullOrWhiteSpace(p.AgeGroup), () =>
        {
            RuleFor(n => n.AgeGroup)
                .Must(ageGroup => ageGroup!.All(char.IsLetter)).WithMessage("{PropertyName} must contain only letters.")
                .Must(ageGroup => AgeRanges.GetValue(ageGroup!) != null)
                .WithMessage("{PropertyName} must be a valid group.");
        });

        When(p => !string.IsNullOrWhiteSpace(p.CountryId), () =>
        {
            RuleFor(n => n.CountryId)
                .Must(countryId => countryId!.All(char.IsLetter)).WithMessage("{PropertyName} must contain only letters.")
                .Length(3).WithMessage("{PropertyName} must be a valid ISO 3166-1 alpha-2 or alpha-3 country code.");
        });
    }
}

public sealed record GetAllProfileQueryHandler(
    ProfileDbContext DbContext,
    ILogger<GetAllProfileQueryHandler> Logger) : IQueryHandler<GetAllProfilesQuery, Result<IProfileResponse>>
{
    public async Task<Result<IProfileResponse>> Handle(GetAllProfilesQuery query, CancellationToken cancellationToken)
    {
        var validator = new GetAllProfilesQueryValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return await Result<IProfileResponse>.FailureAsync(new ErrorResponse("error", validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed"));
        }

        var queryable = DbContext.Profiles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Gender))
            queryable = queryable.Where(p => p.Gender == query.Gender);

        if (!string.IsNullOrWhiteSpace(query.CountryId))
            queryable = queryable.Where(p => p.CountryId == query.CountryId);

        if (!string.IsNullOrWhiteSpace(query.AgeGroup))
            queryable = queryable.Where(p => p.AgeGroup == query.AgeGroup);

        var profiles = await queryable.Select(p => ProfileResponse.FromProfile(p)).ToListAsync(cancellationToken);

        return await Result<IProfileResponse>.SuccessAsync(new ProfileListResponse("success", profiles.Count, profiles));
    }
}