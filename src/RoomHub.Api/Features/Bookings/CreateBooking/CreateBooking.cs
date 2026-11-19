using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees);

public sealed record BookingCreatedResponse(Guid Id, Guid RoomId, string Status);

public sealed class CreateBookingHandler(
    RoomRepository rooms,
    BookingRepository bookings,
    TimeProvider clock)
    : ICommandHandler<CreateBookingCommand, Booking>
{
    public Task<Result<Booking>> Handle(
        CreateBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var room = rooms.Get(command.RoomId);
        if (room is null)
            return Task.FromResult<Result<Booking>>(RoomErrors.NotFound);

        var result = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            clock.GetUtcNow().UtcDateTime);

        if (result.IsFailure)
            return Task.FromResult(result);

        foreach (var existing in bookings.ListForRoom(command.RoomId))
        {
            if (existing.StartUtc <= command.EndUtc && command.StartUtc <= existing.EndUtc)
                return Task.FromResult<Result<Booking>>(BookingErrors.RoomOccupied);
        }

        bookings.Add(result.Value);
        return Task.FromResult(result);
    }
}

public static class CreateBookingEndpoint
{
    public static void MapCreateBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings", async (
            Guid roomId,
            CreateBookingRequest request,
            ICommandHandler<CreateBookingCommand, Booking> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateBookingCommand(
                roomId,
                request.BookedBy,
                request.StartUtc,
                request.EndUtc,
                request.Attendees);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(booking => Results.Created(
                $"/api/bookings/{booking.Id}",
                new BookingCreatedResponse(
                    booking.Id, booking.RoomId, booking.Status.ToString())));
        }).WithTags("Bookings");
    }
}

public sealed record CreateBookingRequest(
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees);
