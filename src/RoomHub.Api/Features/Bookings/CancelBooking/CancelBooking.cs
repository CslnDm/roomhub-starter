using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid Id);

public sealed class CancelBookingHandler(BookingRepository bookings)
    : ICommandHandler<CancelBookingCommand, Unit>
{
    public Task<Result<Unit>> Handle(
        CancelBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var booking = bookings.Get(command.Id);
        if (booking is null)
            return Task.FromResult<Result<Unit>>(BookingErrors.NotFound);

        return Task.FromResult(booking.Cancel());
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
