using HngStageOne.Application.Features.Agify.Responses;
using HngStageOne.Application.Features.Genderize.Responses;
using HngStageOne.Application.Features.Nationalize.Responses;
using HngStageOne.Application.Utilities;

namespace HngStageOne.Application.Abstractions.Services;

public interface IGenderizeClient
{
    Task<Result<GenderizeResponse?>> GetGenderByPersonNameAsync(string name, CancellationToken cancellationToken = default);
}

public interface IAgifyClient
{
    Task<Result<AgifyResponse?>> GetAgeByPersonNameAsync(string name, CancellationToken cancellationToken = default);
}

public interface INationalizeClient
{
    Task<Result<NationalizeResponse?>> GetCountryByPersonNameAsync(string name, CancellationToken cancellationToken = default);
}
