using RoomHub.Api.Domain.Common;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingRequest(
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees);

public sealed record BookingCreatedResponse(Guid Id, Guid RoomId, string Status);

public sealed class CreateBookingHandler(RoomRepository rooms, BookingRepository bookings)
{
    public Result<Booking> Handle(Guid roomId, CreateBookingRequest request)
    {
        var room = rooms.Get(roomId);
        if (room is null)
            return Error.NotFound("RoomNotFound", "The room does not exist.");

        if (string.IsNullOrWhiteSpace(request.BookedBy))
            return Error.Validation("InvalidBookedBy", "BookedBy is required.");

        if (request.EndUtc <= request.StartUtc)
            return Error.Validation("InvalidRange", "The end must be after the start.");

        if (request.Attendees <= 0)
            return Error.Validation("InvalidAttendees", "Attendees must be positive.");

        if (request.Attendees > room.Capacity)
            return Error.Validation("AttendeesExceedCapacity", "The room does not have enough seats.");

        foreach (var existing in bookings.ListForRoom(roomId))
        {
            if (existing.StartUtc <= request.EndUtc && request.StartUtc <= existing.EndUtc)
                return Error.Conflict("RoomOccupied", "The room is occupied in this interval.");
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            BookedBy = request.BookedBy.Trim(),
            StartUtc = request.StartUtc,
            EndUtc = request.EndUtc,
            Attendees = request.Attendees,
            Status = BookingStatus.Active
        };

        bookings.Add(booking);
        return booking;
    }
}

public static class CreateBookingEndpoint
{
    public static void MapCreateBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings",
            (Guid roomId, CreateBookingRequest request, CreateBookingHandler handler) =>
            {
                var result = handler.Handle(roomId, request);
                if (result.IsFailure)
                    return result.Error.ToProblemDetails();

                var booking = result.Value;
                return Results.Created(
                    $"/api/bookings/{booking.Id}",
                    new BookingCreatedResponse(
                        booking.Id, booking.RoomId, booking.Status.ToString()));
            }).WithTags("Bookings");
    }
}
