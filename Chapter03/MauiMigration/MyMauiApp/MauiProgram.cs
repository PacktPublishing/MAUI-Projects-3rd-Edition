using Microsoft.Extensions.Logging;
using MauiMigration.Services;
using MauiMigration.Models;

namespace MyMauiApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            DependencyService.RegisterSingleton<IDataStore<Item>>(new MockDataStore());

#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}