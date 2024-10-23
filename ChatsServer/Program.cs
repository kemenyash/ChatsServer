using ChatsServer.Hubs;
using ChatsServer.Services;
using DataStore;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR services
builder.Services.AddSignalR();

// Add SQLite DbContext before building the app
var sqlitePath = builder.Configuration.GetSection("SQLitePath").Value;
var directory = Path.GetDirectoryName(sqlitePath);
Environment.SetEnvironmentVariable("TelegramToken", builder.Configuration.GetSection("TelegramToken").Value);

if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite($"Data Source={sqlitePath}")
           .LogTo(Console.WriteLine, LogLevel.Warning),
    ServiceLifetime.Scoped);

builder.Services.AddScoped<DataScope>();
builder.Services.AddSingleton<TelegramBotService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    var botService = scope.ServiceProvider.GetService<TelegramBotService>();

    dbContext.Database.Migrate();
    botService.Invoke();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add routing middleware before endpoints
app.UseRouting();

app.UseAuthorization();

// Map the SignalR hubs and other endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatsHub>("/chatsHub");
    endpoints.MapControllers();
});

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
