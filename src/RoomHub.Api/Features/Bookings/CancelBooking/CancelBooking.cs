using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid Id);

public sealed class CancelBookingHandler(
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
    : ICommandHandler<CancelBookingCommand, Unit>
{
    public async Task<Result<Unit>> Handle(
        CancelBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var booking = await bookings.GetAsync(command.Id, cancellationToken);
        if (booking is null)
            return BookingErrors.NotFound;

        var result = booking.Cancel(clock.GetUtcNow().UtcDateTime);
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}

public static class CancelBookingEndpoint
{
    public static void MapCancelBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{id:guid}/cancel", async (
            Guid id,
            ICommandHandler<CancelBookingCommand, Unit> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new CancelBookingCommand(id),
                cancellationToken);
            return result.Match(_ => Results.NoContent());
        }).WithTags("Bookings");
    }
}
