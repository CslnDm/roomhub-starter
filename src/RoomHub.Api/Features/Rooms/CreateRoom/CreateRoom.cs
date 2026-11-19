using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(string? Name, int Capacity);

public sealed record RoomResponse(Guid Id, string Name, int Capacity);

public sealed class CreateRoomHandler(
    IRoomRepository rooms,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoomCommand, Room>
{
    public Task<Result<Room>> Handle(
        CreateRoomCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = Room.Create(command.Name, command.Capacity);
        if (result.IsSuccess)
        {
            rooms.Add(result.Value);
            return SaveAsync(result, cancellationToken);
        }

        return Task.FromResult(result);
    }

    private async Task<Result<Room>> SaveAsync(
        Result<Room> result,
        CancellationToken cancellationToken)
    {
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
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
