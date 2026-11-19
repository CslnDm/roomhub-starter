using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Common;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Error error) =>
        Results.Problem(
            statusCode: error.StatusCode,
            title: error.Code,
            detail: error.Message);
}
