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

// Configuración de Cookies seguras
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");

    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self' https://adivinalapalabra-fnb3.onrender.com; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval' " +
                "https://adivinalapalabra-fnb3.onrender.com " +
                "https://kit.fontawesome.com " +
                "https://ka-f.fontawesome.com " +
                "https://ajax.googleapis.com " +
                "https://cdn.jsdelivr.net " +
                "https://cdnjs.cloudflare.com; " +
            "style-src 'self' 'unsafe-inline' " +
                "https://adivinalapalabra-fnb3.onrender.com " +
                "https://ka-f.fontawesome.com; " +
            "font-src 'self' https://ka-f.fontawesome.com data:; " +
            "img-src 'self' data: https://adivinalapalabra-fnb3.onrender.com; " +
            "connect-src 'self' https://adivinalapalabra-fnb3.onrender.com " +
                "https://api.example.com " +
                "https://ka-f.fontawesome.com " +
                "wss://adivinalapalabra-fnb3.onrender.com " +
                "https://cdnjs.cloudflare.com; " +
            "frame-src 'self'; " +
            "object-src 'none'; " +
            "base-uri 'none';");
    }

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
