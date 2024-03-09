using BetModels.Interfaces;
using CommunityToolkit.Maui;
using MauiTelesportApp.Data;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Scarp;
using SendToShop.Output.Text;
using SendToShop.SignalR;
namespace MauiWinnerApp;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();
        builder.Services.AddTransient<IScarp, Scarping2>();

        builder.Services.AddTransient<ITextService, TextGenerator>();
        builder.Services.AddTransient<IFileSystemService, FileSystemService>();
        builder.Services.AddSingleton<IFormSender, SignalRFormSender>();
        builder.Services.AddTransient<IPreferencesService, PreferencesService>();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
