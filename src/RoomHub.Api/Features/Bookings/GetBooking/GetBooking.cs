using RoomHub.Api.Domain.Common;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record BookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public sealed class GetBookingHandler(BookingRepository bookings)
{
    public Result<Booking> Handle(Guid id)
    {
        var booking = bookings.Get(id);
        if (booking is null)
            return Error.NotFound("BookingNotFound", "The booking does not exist.");

        return booking;
    }
}

public static class GetBookingEndpoint
{
    public static void MapGetBooking(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/{id:guid}", (Guid id, GetBookingHandler handler) =>
        {
            var result = handler.Handle(id);
            if (result.IsFailure)
                return result.Error.ToProblemDetails();

            var booking = result.Value;
            return Results.Ok(new BookingResponse(
                booking.Id,
                booking.RoomId,
                booking.BookedBy,
                booking.StartUtc,
                booking.EndUtc,
                booking.Attendees,
                booking.Status.ToString()));
        }).WithTags("Bookings");
    }
}
