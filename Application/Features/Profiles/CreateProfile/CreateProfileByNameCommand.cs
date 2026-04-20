using Cortex.Mediator.Commands;
using FluentValidation;
using HngStageOne.Application.Abstractions.Services;
using HngStageOne.Application.Features.Profiles.Responses;
using HngStageOne.Application.Utilities;
using HngStageOne.Domain.Models;
using HngStageOne.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HngStageOne.Application.Features.Profiles.CreateProfile;

public class CreateProfileValidator : AbstractValidator<CreateProfileByNameCommand>
{
    public CreateProfileValidator()
    {
        RuleFor(n => n.Name)
            .NotEmpty().WithMessage("{PropertyName} is Required.")
            .Must(name => name.All(char.IsLetter)).WithMessage("{PropertyName} must contain only letters.")
            .MaximumLength(255).WithMessage("{PropertyName} cannot exceed {MaxLength} characters.");
    }
}

public sealed record CreateProfileByNameCommand(string Name) : ICommand<Result<IProfileResponse>>;

public sealed record CreateProfileByNameCommandHandler(
    ProfileDbContext DbContext,
    IGenderizeClient GenderizeClient,
    IAgifyClient AgifyClient,
    INationalizeClient NationalizeClient
    ) : ICommandHandler<CreateProfileByNameCommand, Result<IProfileResponse>>
{

    public async Task<Result<IProfileResponse>> Handle(CreateProfileByNameCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateProfileValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return await Result<IProfileResponse>.FailureAsync(new ErrorResponse("error", validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed"));
        }

        var existingProfile = await DbContext.Profiles.FirstOrDefaultAsync(p => p.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (existingProfile != null)
        {
            return await Result<IProfileResponse>.SuccessAsync(new DuplicateResponse("Profile already exists", "success", ProfileResponse.FromProfile(existingProfile)));
        }

        var genderizeResult = await GenderizeClient.GetGenderByPersonNameAsync(request.Name, cancellationToken);

        if (!genderizeResult.Succeeded)
        {
            return await Result<IProfileResponse>.FailureAsync(genderizeResult.Error!);
        }

        var agifyResult = await AgifyClient.GetAgeByPersonNameAsync(request.Name, cancellationToken);
        if (!agifyResult.Succeeded)
        {
            return await Result<IProfileResponse>.FailureAsync(agifyResult.Error!);
        }

        var nationalizeResult = await NationalizeClient.GetCountryByPersonNameAsync(request.Name, cancellationToken);
        if (!nationalizeResult.Succeeded)
        {
            return await Result<IProfileResponse>.FailureAsync(nationalizeResult.Error!);
        }

        var maxProbCountry = nationalizeResult.Data!.Country!.OrderByDescending(c => c.Probability).FirstOrDefault();

        var profile = new Profile
        {
            Name = request.Name,
            Age = agifyResult.Data!.Age!.Value,
            AgeGroup = AgeRanges.GetKey(agifyResult.Data.Age!.Value)!,
            CountryId = maxProbCountry!.CountryId!,
            CountryProbability = maxProbCountry!.Probability!,
            CreatedAt = DateTime.UtcNow,
            Gender = genderizeResult.Data!.Gender!,
            GenderProbability = genderizeResult.Data.Probability,
            SampleSize = genderizeResult.Data.Count,
            Id = Guid.CreateVersion7()
        };
        var result = await DbContext.AddAsync(profile, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return await Result<IProfileResponse>.SuccessAsync(new SuccessResponse("success", ProfileResponse.FromProfile(profile)));
    }
}
