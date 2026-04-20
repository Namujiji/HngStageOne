using HngStageOne.Application.Features.Profiles.Responses;

namespace HngStageOne.Application.Utilities;

public interface IResult<T>
{
    bool Succeeded { get; set; }

    T? Data { get; set; }
}

public class Result<T> : IResult<T>
{
    public bool Succeeded { get; set; }

    public T? Data { get; set; }

    public ErrorResponse? Error { get; set; }

    public static Task<Result<T>> SuccessAsync(T data)
    {
        return Task.FromResult(new Result<T>
        {
            Succeeded = true,
            Data = data
        });
    }

    public static Task<Result<T>> FailureAsync(ErrorResponse data)
    {
        return Task.FromResult(new Result<T>
        {
            Succeeded = false,
            Error = data
        });
    }
}