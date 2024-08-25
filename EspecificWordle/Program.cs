using EspecificWordle.Interfaces;
using EspecificWordle.Models.ConfigApp;
using EspecificWordle.Services;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IWordleService, WordleService>();
builder.Services.AddSingleton<ConfigApp>();

// Hangfire
builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

var app = builder.Build();

async Task InitializeConfigApp(IServiceProvider services)
{
    var configApp = services.GetRequiredService<ConfigApp>();
    var wordService = services.GetRequiredService<IWordleService>();

    // Ir agregando los demas modos acá
    configApp.RandomWord = (await wordService.GetAleatoriaAsync()).Palabra;
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitializeConfigApp(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Wordle}/{action=Index}/{id?}");

app.Run();
