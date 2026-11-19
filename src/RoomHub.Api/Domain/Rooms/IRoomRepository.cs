using RoomHub.Api.Models;

namespace RoomHub.Api.Domain.Rooms;

public interface IRoomRepository
{
    Task<Room?> GetAsync(Guid id, CancellationToken cancellationToken);

    void Add(Room room);
}
