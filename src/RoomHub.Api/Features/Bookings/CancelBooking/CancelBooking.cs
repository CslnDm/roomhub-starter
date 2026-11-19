using RoomHub.Api.Domain.Common;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed class CancelBookingHandler(BookingRepository bookings)
{
    public Result<bool> Handle(Guid id)
    {
        var booking = bookings.Get(id);
        if (booking is null)
            return Error.NotFound("BookingNotFound", "The booking does not exist.");

        booking.Status = BookingStatus.Cancelled;
        return true;
    }
}

public static class CancelBookingEndpoint
{
    public static void MapCancelBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{id:guid}/cancel", (Guid id, CancelBookingHandler handler) =>
        {
            var result = handler.Handle(id);
            if (result.IsFailure)
                return result.Error.ToProblemDetails();

            return Results.NoContent();
        }).WithTags("Bookings");
    }
}
