using Microsoft.Extensions.Logging;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using DriverLogisticsApp;

namespace DriverLogisticsApp
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // services
            builder.Services.AddSingleton<DatabaseService>();

            // view models
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<AddLoadViewModel>();

            // views/pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AddLoadViewModel>();

            return builder.Build();
        }
    }
}
