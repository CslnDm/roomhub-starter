using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Models;

namespace RoomHub.Api.Infrastructure;

public sealed class SqliteBookingRepository(RoomHubDbContext context) : IBookingRepository
{
    public Task<Booking?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        context.Bookings.SingleOrDefaultAsync(booking => booking.Id == id, cancellationToken);

    public Task<bool> HasOverlapAsync(
        Guid roomId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken cancellationToken) =>
        context.Bookings.AnyAsync(
            b => b.RoomId == roomId
                && b.Status == BookingStatus.Active
                && b.StartUtc < endUtc
                && startUtc < b.EndUtc,
            cancellationToken);

    public void Add(Booking booking) => context.Bookings.Add(booking);
}
