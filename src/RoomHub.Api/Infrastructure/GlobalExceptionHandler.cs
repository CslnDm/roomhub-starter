using Microsoft.AspNetCore.Diagnostics;

namespace RoomHub.Api.Infrastructure;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unexpected error occurred.");
        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError)
            .ExecuteAsync(httpContext);
        return true;
    }
}
