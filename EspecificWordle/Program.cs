using DataBase.Models;
using EspecificWordle.Hubs;
using EspecificWordle.Interfaces;
using EspecificWordle.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IWordleService, WordleService>();
builder.Services.AddSingleton<IRefreshService, RefreshService>();

// Hangfire
builder.Services.AddHangfire(config => config.UsePostgreSqlStorage(
    builder.Configuration.GetConnectionString("DatabaseConnection")
));
builder.Services.AddHangfireServer();

// SignalR
builder.Services.AddSignalR();

// Conexión a la BD
builder.Services.AddDbContext<WordGameContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection"))
);

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// Manejo de excepciones
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Encabezados
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy", "form-action 'self'; frame-ancestors 'none';");
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");

    await next();
});

// Autorización y Hangfire Dashboard
app.UseAuthorization();
app.UseHangfireDashboard();

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

    // Hangfire: Programación de tareas
    recurringJobManager.AddOrUpdate("UpdateRandomWordDaily", () => wordService.UpdateRandomWordDaily(), "0 3 * * *");
    recurringJobManager.AddOrUpdate("NotifyRefresh", () => refreshService.NotifyRefresh(), "0 3 * * *");
}

// Rutas y SignalR
app.MapControllers();
app.MapHub<RefreshHub>("/refreshHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
);

app.Run();
