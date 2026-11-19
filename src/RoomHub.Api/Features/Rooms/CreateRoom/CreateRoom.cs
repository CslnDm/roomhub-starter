using RoomHub.Api.Domain.Common;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomRequest(string? Name, int Capacity);

public sealed record RoomResponse(Guid Id, string Name, int Capacity);

public sealed class CreateRoomHandler(RoomRepository rooms)
{
    public Result<Room> Handle(CreateRoomRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Error.Validation("InvalidName", "Room name is required.");

        if (request.Capacity <= 0)
            return Error.Validation("InvalidCapacity", "Capacity must be positive.");

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Capacity = request.Capacity
        };

        rooms.Add(room);
        return room;
    }
}

public static class CreateRoomEndpoint
{
    public static void MapCreateRoom(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms", (CreateRoomRequest request, CreateRoomHandler handler) =>
        {
            var result = handler.Handle(request);
            if (result.IsFailure)
                return result.Error.ToProblemDetails();

            var room = result.Value;
            return Results.Created(
                $"/api/rooms/{room.Id}",
                new RoomResponse(room.Id, room.Name, room.Capacity));
        }).WithTags("Rooms");
    }
}
