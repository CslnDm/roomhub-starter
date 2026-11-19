using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees);

public sealed record BookingCreatedResponse(Guid Id, Guid RoomId, string Status);

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
    : ICommandHandler<CreateBookingCommand, Booking>
{
    public async Task<Result<Booking>> Handle(
        CreateBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var room = await rooms.GetAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        var result = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            clock.GetUtcNow().UtcDateTime);

        if (result.IsFailure)
            return result;

        if (await bookings.HasOverlapAsync(
                command.RoomId,
                command.StartUtc,
                command.EndUtc,
                cancellationToken))
            return BookingErrors.RoomOccupied;

        var dayStartUtc = command.StartUtc.Date;
        var dayEndUtc = dayStartUtc.AddDays(1);
        var activeCount = await bookings.CountActiveByUserOnDayAsync(
            command.BookedBy!.Trim(),
            dayStartUtc,
            dayEndUtc,
            cancellationToken);
        if (activeCount >= 2)
            return BookingErrors.DailyLimitExceeded;

        bookings.Add(result.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
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
