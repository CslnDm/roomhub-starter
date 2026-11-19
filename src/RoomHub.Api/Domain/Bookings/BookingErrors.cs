using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class BookingErrors
{
    public static readonly Error InvalidBookedBy =
        Error.Validation("Booking.InvalidBookedBy", "BookedBy is required.");

    public static readonly Error InvalidRange =
        Error.Validation("Booking.InvalidRange", "The end must be after the start.");

    public static readonly Error InvalidAttendees =
        Error.Validation("Booking.InvalidAttendees", "Attendees must be positive.");

    public static readonly Error AttendeesExceedCapacity =
        Error.Validation("Booking.AttendeesExceedCapacity", "The room does not have enough seats.");

    public static readonly Error StartsInPast =
        Error.Validation("Booking.StartsInPast", "The booking cannot start in the past.");

    public static readonly Error TooLong =
        Error.Validation("Booking.TooLong", "The booking cannot be longer than 4 hours.");

    public static readonly Error TooLongForLargeRoom =
        Error.Validation("Booking.TooLongForLargeRoom", "A large room booking cannot be longer than 2 hours.");

    public static readonly Error DailyLimitExceeded =
        Error.Conflict("Booking.DailyLimitExceeded", "The participant already has 2 active bookings on this day.");

    public static readonly Error CancelTooLate =
        Error.Conflict("Booking.CancelTooLate", "The booking can only be cancelled at least 60 minutes before it starts.");

    public static readonly Error NotFound =
        Error.NotFound("Booking.NotFound", "The booking does not exist.");

    public static readonly Error AlreadyCancelled =
        Error.Conflict("Booking.AlreadyCancelled", "The booking is already cancelled.");

    public static readonly Error RoomOccupied =
        Error.Conflict("Booking.RoomOccupied", "The room is occupied in this interval.");
}
