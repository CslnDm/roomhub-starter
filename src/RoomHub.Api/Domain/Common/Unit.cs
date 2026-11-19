namespace RoomHub.Api.Domain.Common;

public sealed class Unit
{
    private Unit()
    {
    }

    public static Unit Value { get; } = new();
}
