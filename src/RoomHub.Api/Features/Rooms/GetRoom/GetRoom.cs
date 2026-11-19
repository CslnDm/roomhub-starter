using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomQuery(Guid Id);

public sealed record RoomResponse(Guid Id, string Name, int Capacity);

public sealed class GetRoomHandler(RoomRepository rooms)
    : IQueryHandler<GetRoomQuery, Room>
{
    public Task<Result<Room>> Handle(
        GetRoomQuery query,
        CancellationToken cancellationToken = default)
    {
        var room = rooms.Get(query.Id);
        var result = room is null
            ? RoomErrors.NotFound
            : Result<Room>.Success(room);

        return Task.FromResult(result);
    }
}

public static class GetRoomEndpoint
{
    public static void MapGetRoom(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/rooms/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRoomQuery, Room> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetRoomQuery(id), cancellationToken);
            return result.Match(room => Results.Ok(
                new RoomResponse(room.Id, room.Name, room.Capacity)));
        }).WithTags("Rooms");
    }
}
