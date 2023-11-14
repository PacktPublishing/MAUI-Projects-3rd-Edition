using Microsoft.Extensions.Logging;

namespace MeTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<Services.ILocationTrackingService, Services.LocationTrackingService>();
        builder.Services.AddSingleton<Repositories.ILocationRepository, Repositories.LocationRepository>();

        builder.Services.AddTransient(typeof(ViewModels.MainViewModel));
        builder.Services.AddTransient(typeof(Views.MainView));

        return builder.Build();
    }
}
