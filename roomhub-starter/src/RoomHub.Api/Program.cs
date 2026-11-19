using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Enums travel as text in JSON ("Active", "Cancelled").
builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Errors that do not come from a handler (bad JSON, unexpected exceptions) are also ProblemDetails.
builder.Services.AddProblemDetails();
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

// "Now" comes from TimeProvider, never from DateTime.UtcNow (R5).
builder.Services.AddSingleton(TimeProvider.System);

// The DbContext is Scoped: one instance per HTTP request.
builder.Services.AddDbContext<RoomHubDbContext>((serviceProvider, options) =>
    options.UseSqlite(
        serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("RoomHub")
        ?? "Data Source=roomhub.db"));

// Repositories and the Unit of Work are Scoped and share the same DbContext within a request.
builder.Services.AddScoped<IUnitOfWork>(serviceProvider =>
    serviceProvider.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Every ICommandHandler / IQueryHandler in the assembly, registered once at startup.
builder.Services.AddHandlers(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

await app.InitializeDatabaseAsync();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRooms();
app.MapBookings();

app.Run();

// Lets integration tests use WebApplicationFactory<Program>.
public partial class Program
{
}
