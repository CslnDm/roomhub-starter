using RoomHub.Api.Domain.Common;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record RoomResponse(Guid Id, string Name, int Capacity);

public sealed class GetRoomHandler(RoomRepository rooms)
{
    public Result<Room> Handle(Guid id)
    {
        var room = rooms.Get(id);
        if (room is null)
            return Error.NotFound("RoomNotFound", "The room does not exist.");

        return room;
    }
}

public static class GetRoomEndpoint
{
    public static void MapGetRoom(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/rooms/{id:guid}", (Guid id, GetRoomHandler handler) =>
        {
            var result = handler.Handle(id);
            if (result.IsFailure)
                return result.Error.ToProblemDetails();

            var room = result.Value;
            return Results.Ok(new RoomResponse(room.Id, room.Name, room.Capacity));
        }).WithTags("Rooms");
    }
}
