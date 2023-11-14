namespace GalleryApp;

using GalleryApp.Services;
using Microsoft.Extensions.Logging;

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

        builder.Services.AddSingleton<IPhotoImporter>(serviceProvider => new PhotoImporter());
        builder.Services.AddTransient<ILocalStorage>(ServiceProvider => new MauiLocalStorage());

        builder.Services.AddTransient<ViewModels.MainViewModel>();
        builder.Services.AddTransient<ViewModels.GalleryViewModel>();

        builder.Services.AddTransient<Views.MainView>();
        builder.Services.AddTransient<Views.GalleryView>();

        return builder.Build();
	}
}
