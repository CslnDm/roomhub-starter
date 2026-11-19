using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Error error) => error.Type switch
    {
        ErrorType.Validation => TypedResults.Problem(error.Message, statusCode: 400, title: error.Code),
        ErrorType.NotFound => TypedResults.Problem(error.Message, statusCode: 404, title: error.Code),
        ErrorType.Conflict => TypedResults.Problem(error.Message, statusCode: 409, title: error.Code),
        _ => TypedResults.Problem(statusCode: 500)
    };

    public static IResult Match<T>(this Result<T> result, Func<T, IResult> onSuccess) =>
        result.IsSuccess ? onSuccess(result.Value) : result.Error.ToHttpResult();
}
