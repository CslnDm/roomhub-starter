using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid Id);

public sealed record BookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public sealed class GetBookingHandler(BookingRepository bookings)
    : IQueryHandler<GetBookingQuery, Booking>
{
    public Task<Result<Booking>> Handle(
        GetBookingQuery query,
        CancellationToken cancellationToken = default)
    {
        var booking = bookings.Get(query.Id);
        var result = booking is null
            ? BookingErrors.NotFound
            : Result<Booking>.Success(booking);

        return Task.FromResult(result);
    }
}

public static class GetBookingEndpoint
{
    public static void MapGetBooking(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBookingQuery, Booking> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetBookingQuery(id), cancellationToken);
            return result.Match(booking => Results.Ok(new BookingResponse(
                booking.Id,
                booking.RoomId,
                booking.BookedBy,
                booking.StartUtc,
                booking.EndUtc,
                booking.Attendees,
                booking.Status.ToString())));
        }).WithTags("Bookings");
    }
}
