namespace HngStageOne.Application.Features.Profiles.Responses;

public record ProfileListResponse(string Status, int Count, List<ProfileResponse> Data) : IProfileResponse;
