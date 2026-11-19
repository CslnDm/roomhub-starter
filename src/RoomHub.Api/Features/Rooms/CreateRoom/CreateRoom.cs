using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(string? Name, int Capacity);

public sealed record RoomResponse(Guid Id, string Name, int Capacity);

public sealed class CreateRoomHandler(RoomRepository rooms)
    : ICommandHandler<CreateRoomCommand, Room>
{
    public Task<Result<Room>> Handle(
        CreateRoomCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = Room.Create(command.Name, command.Capacity);
        if (result.IsSuccess)
            rooms.Add(result.Value);

        return Task.FromResult(result);
    }
}

public static class CreateRoomEndpoint
{
    public static void MapCreateRoom(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms", async (
            CreateRoomCommand command,
            ICommandHandler<CreateRoomCommand, Room> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(room => Results.Created(
                $"/api/rooms/{room.Id}",
                new RoomResponse(room.Id, room.Name, room.Capacity)));
        }).WithTags("Rooms");
    }
}
