using ChatsServer.Hubs;
using ChatsServer.Services;
using DataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new EscapingStringConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();


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
builder.Services.AddScoped<ProtectionService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    var botService = scope.ServiceProvider.GetService<TelegramBotService>();
    var protectionService = scope.ServiceProvider.GetService<ProtectionService>();

    dbContext.Database.Migrate();
    botService.Invoke();
    protectionService.Invoke().ConfigureAwait(false);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseRouting();
app.UseStaticFiles();

app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatsHub>("/chatsHub");
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();
