using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Models;

namespace RoomHub.Api.Infrastructure;

public sealed class RoomHubDbContext(DbContextOptions<RoomHubDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Booking> Bookings => Set<Booking>();

    Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            value => value,
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(room => room.Id);
            entity.Property(room => room.Name)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(booking => booking.Id);
            entity.Property(booking => booking.BookedBy)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(booking => booking.Status)
                .HasConversion<string>()
                .IsRequired();
            entity.Property(booking => booking.StartUtc)
                .HasConversion(utcConverter)
                .IsRequired();
            entity.Property(booking => booking.EndUtc)
                .HasConversion(utcConverter)
                .IsRequired();
            entity.HasIndex(booking => new
            {
                booking.RoomId,
                booking.StartUtc,
                booking.EndUtc
            });
        });
    }
}
