namespace HngStageOne.Application.Features.Profiles.Responses;

public record ErrorResponse(
    string Status,
    string Message
) : IProfileResponse;

public record SuccessResponse(
    string Status,
    ProfileResponse Data
) : IProfileResponse;

public record DuplicateResponse(string Message, string Status, ProfileResponse Data) : SuccessResponse(Status, Data);

public interface IProfileResponse : IApiResponse
{
    string Status { get; init; }
}

public interface IApiResponse;