
using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;
using RoomHub.Api.Infrastructure;
using RoomHub.Api.Repositories;
using System.Text.Json.Serialization;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RoomRepository>();
builder.Services.AddSingleton<BookingRepository>();
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

app.UseExceptionHandler();
app.MapGet("/", () => "RoomHub API is running.");
app.MapCreateRoom();
app.MapGetRoom();
app.MapCreateBooking();
app.MapGetBooking();
app.MapCancelBooking();

app.Run();
