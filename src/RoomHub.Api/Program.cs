
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Infrastructure;
using System.Text.Json.Serialization;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Api-Version"));
});
var connectionString = builder.Configuration.GetConnectionString("RoomHub")
    ?? "Data Source=roomhub.db";
builder.Services.AddDbContext<RoomHubDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<IUnitOfWork>(services =>
    services.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, SqliteRoomRepository>();
builder.Services.AddScoped<IBookingRepository, SqliteBookingRepository>();
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddScoped<ICommandHandler<CreateRoomCommand, Room>, CreateRoomHandler>();
builder.Services.AddScoped<IQueryHandler<GetRoomQuery, Room>, GetRoomHandler>();
builder.Services.AddScoped<ICommandHandler<CreateBookingCommand, Booking>, CreateBookingHandler>();
builder.Services.AddScoped<IQueryHandler<GetBookingQuery, Booking>, GetBookingHandler>();
builder.Services.AddScoped<ICommandHandler<CancelBookingCommand, Unit>, CancelBookingHandler>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider
        .GetRequiredService<RoomHubDbContext>()
        .Database
        .EnsureCreated();
}

app.UseExceptionHandler();
app.MapGet("/", () => "RoomHub API is running.");
app.MapCreateRoom();
app.MapGetRoom();
app.MapCreateBooking();
app.MapGetBooking();
app.MapCancelBooking();

app.Run();
