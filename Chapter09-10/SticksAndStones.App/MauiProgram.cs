using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace SticksAndStones.App
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
                    fonts.AddFont("FontAwesome.otf", "FontAwesome");
                })
                .UseMauiCommunityToolkit();

#if DEBUG
            builder.Logging.AddDebug();

            builder.Services.AddLogging(configure =>
            {
                configure.AddDebug();
            });
#endif
            builder.Services.AddSingleton<Services.Settings>();
            builder.Services.AddSingleton<Services.ServiceConnection>();

            builder.Services.AddSingleton<Services.GameService>();

            builder.Services.AddTransient<ViewModels.ConnectViewModel>();
            builder.Services.AddTransient<ViewModels.LobbyViewModel>();
            builder.Services.AddTransient<ViewModels.MatchViewModel>();

            builder.Services.AddTransient<Views.ConnectView>();
            builder.Services.AddTransient<Views.LobbyView>();
            builder.Services.AddTransient<Views.MatchView>();

            return builder.Build();
        }
    }
}