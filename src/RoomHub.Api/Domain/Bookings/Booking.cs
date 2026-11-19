using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Models;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed class Booking
{
    private Booking()
    {
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime nowUtc)
    {
        var error = Validate(room, bookedBy, startUtc, endUtc, attendees, nowUtc);
        if (error is not null)
            return error;

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = bookedBy!.Trim(),
            StartUtc = startUtc,
            EndUtc = endUtc,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    public Result<Unit> Cancel(DateTime nowUtc)
    {
        if (StartUtc - nowUtc < TimeSpan.FromMinutes(60))
            return BookingErrors.CancelTooLate;

        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }

    private static Error? Validate(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.InvalidBookedBy;

        if (endUtc <= startUtc)
            return BookingErrors.InvalidRange;

        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees > room.Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        if (startUtc < nowUtc)
            return BookingErrors.StartsInPast;

        if (endUtc - startUtc > room.MaxBookingDuration)
            return room.Capacity > 10
                ? BookingErrors.TooLongForLargeRoom
                : BookingErrors.TooLong;

        return null;
    }
}
