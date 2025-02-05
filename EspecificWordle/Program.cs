using DataBase.Models;
using EspecificWordle.Hubs;
using EspecificWordle.Interfaces;
using EspecificWordle.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IWordleService, WordleService>();
builder.Services.AddSingleton<IRefreshService, RefreshService>();

// Hangfire
builder.Services.AddHangfire(config => config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.AddHangfireServer();

// SignalR
builder.Services.AddSignalR();

// Conexion a la BD
builder.Services.AddDbContext<WordGameContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection"))
);

var app = builder.Build();

// Middleware de manejo de excepciones global
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
app.UseHttpsRedirection();

async Task InitializeConfigApp(IServiceProvider services)
{
    var wordService = services.GetRequiredService<IWordleService>();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitializeConfigApp(services);

    var recurringJobManager = services.GetRequiredService<IRecurringJobManager>();
    var wordService = services.GetRequiredService<IWordleService>();
    var refreshService = services.GetRequiredService<IRefreshService>();

    // Hangfire
    recurringJobManager.AddOrUpdate("UpdateRandomWordDaily", () => wordService.UpdateRandomWordDaily(), "0 3 * * *");
    recurringJobManager.AddOrUpdate("NotifyRefresh", () => refreshService.NotifyRefresh(), "0 3 * * *");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseHangfireDashboard();

// Para SignalR
app.MapControllers();
app.MapHub<RefreshHub>("/refreshHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
