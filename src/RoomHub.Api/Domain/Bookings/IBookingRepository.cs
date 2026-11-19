using RoomHub.Api.Models;

namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> HasOverlapAsync(
        Guid roomId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken cancellationToken);

    void Add(Booking booking);
}
