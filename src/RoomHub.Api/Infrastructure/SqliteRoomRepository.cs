using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Models;

namespace RoomHub.Api.Infrastructure;

public sealed class SqliteRoomRepository(RoomHubDbContext context) : IRoomRepository
{
    public Task<Room?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        context.Rooms.SingleOrDefaultAsync(room => room.Id == id, cancellationToken);

    public void Add(Room room) => context.Rooms.Add(room);
}
