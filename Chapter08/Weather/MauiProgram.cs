using Microsoft.Extensions.Logging;
using Weather.Services;
using Weather.ViewModels;
using Weather.Views;

namespace Weather;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddSingleton<IWeatherService, OpenWeatherMapWeatherService>();

		builder.Services.AddTransient<MainViewModel, MainViewModel>();
		if (DeviceInfo.Idiom == DeviceIdiom.Phone)
		{
			builder.Services.AddTransient<IMainView, Views.Mobile.MainView>();
		}
		else
		{
			builder.Services.AddTransient<IMainView, Views.Desktop.MainView>();
		}

        return builder.Build();
	}
}
