using HngStageOne.Application.Abstractions.Services;
using HngStageOne.Application.Features.Agify.Responses;
using HngStageOne.Application.Features.Genderize.Responses;
using HngStageOne.Application.Features.Nationalize.Responses;
using HngStageOne.Application.Features.Profiles.Responses;
using HngStageOne.Application.Utilities;

namespace HngStageOne.Infrastructure.Services;

public class GenderizeClient(
    HttpClient client) : IGenderizeClient
{
    public async Task<Result<GenderizeResponse?>> GetGenderByPersonNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ErrorResponse error = new("502", "Genderize returned an invalid response");
        try
        {
            var response = await client.GetAsync($"?name={name}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await Result<GenderizeResponse?>.SuccessAsync(await response.Content.ReadFromJsonAsync<GenderizeResponse>(cancellationToken));

                if (result.Data!.Count == 0 || string.IsNullOrWhiteSpace(result.Data!.Gender))
                {
                    return await Result<GenderizeResponse?>.FailureAsync(error);
                }

                return result;
            }

            return await Result<GenderizeResponse?>.FailureAsync(error);


        }
        catch (Exception)
        {
            return await Result<GenderizeResponse?>.FailureAsync(error);
        }

    }
}

public class AgifyClient(HttpClient client) : IAgifyClient
{
    public async Task<Result<AgifyResponse?>> GetAgeByPersonNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ErrorResponse error = new("502", "Agify returned an invalid response");

        try
        {
            var response = await client.GetAsync($"?name={name}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await Result<AgifyResponse?>.SuccessAsync(await response.Content.ReadFromJsonAsync<AgifyResponse>(cancellationToken));

                if (result.Data is null || !result.Data.Age.HasValue)
                {
                    return await Result<AgifyResponse?>.FailureAsync(error);
                }

                return result;
            }

            return await Result<AgifyResponse?>.FailureAsync(error);


        }
        catch (Exception)
        {
            return await Result<AgifyResponse?>.FailureAsync(error);
        }
    }
}

public class NationalizeClient(HttpClient client) : INationalizeClient
{
    public async Task<Result<NationalizeResponse?>> GetCountryByPersonNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ErrorResponse error = new("502", "Nationalize returned an invalid response");

        try
        {
            var response = await client.GetAsync($"?name={name}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await Result<NationalizeResponse?>.SuccessAsync(await response.Content.ReadFromJsonAsync<NationalizeResponse>(cancellationToken));

                if (result.Data is null || result.Data.Country.Count == 0)
                {
                    return await Result<NationalizeResponse?>.FailureAsync(error);
                }

                return result;
            }

            return await Result<NationalizeResponse?>.FailureAsync(error);

        }
        catch (Exception)
        {
            return await Result<NationalizeResponse?>.FailureAsync(error);
        }
    }
}