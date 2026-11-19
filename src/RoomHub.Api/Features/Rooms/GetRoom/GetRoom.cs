using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Common;
using RoomHub.Api.Models;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomQuery(Guid Id);

public sealed record RoomResponse(Guid Id, string Name, int Capacity);

public sealed class GetRoomHandler(IRoomRepository rooms)
    : IQueryHandler<GetRoomQuery, Room>
{
    public Task<Result<Room>> Handle(
        GetRoomQuery query,
        CancellationToken cancellationToken = default)
    {
        return GetAsync(query.Id, cancellationToken);
    }

    private async Task<Result<Room>> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var room = await rooms.GetAsync(id, cancellationToken);
        var result = room is null
            ? RoomErrors.NotFound
            : Result<Room>.Success(room);

        return result;
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
