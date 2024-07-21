using EspecificWordle.Interfaces;
using EspecificWordle.Models.ConfigApp;
using EspecificWordle.Services;
using Python.Runtime;

var builder = WebApplication.CreateBuilder(args);

// Establece la ruta del archivo DLL de Python 3.12
Runtime.PythonDLL = @"C:\Users\USER\AppData\Local\Programs\Python\Python312\python312.dll";

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IWordleService, WordleService>();
builder.Services.AddSingleton<ConfigApp>();

var app = builder.Build();

async Task InitializeConfigApp(IServiceProvider services)
{
    var configApp = services.GetRequiredService<ConfigApp>();
    var wordService = services.GetRequiredService<IWordleService>();
    configApp.RandomWord = await wordService.RandomWordleAsync();
    
    PythonEngine.Initialize();

    using (Py.GIL())
    {
        configApp.RandomWordDef = await wordService.GetDefinitionWord(configApp.RandomWord);
        configApp.RandomWordDefRae = await wordService.GetDefinitionRaeWord(configApp.RandomWord);
        configApp.RandomWordEn = await wordService.TranslateWord(configApp.RandomWord);
        configApp.RandomWordSynonyms = await wordService.GetSynonymsWord(configApp.RandomWord);
        configApp.RandomWordAntonyms = await wordService.GetAntonymsWord(configApp.RandomWord);
        configApp.RandomWordUseExamples= await wordService.GetWordUseExample(configApp.RandomWord);
    }

    // PythonEngine.Shutdown();
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
