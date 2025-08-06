using Microsoft.Extensions.Logging;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using DriverLogisticsApp.Views;
using DriverLogisticsApp.Popups;
using DriverLogisticsApp;
using CommunityToolkit.Maui;

namespace DriverLogisticsApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // services
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<IAlertService, MauiAlertService>();
            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();

            // view models
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<AddLoadViewModel>();
            builder.Services.AddTransient<LoadDetailsViewModel>();
            builder.Services.AddTransient<AddExpenseViewModel>();
            builder.Services.AddTransient<ExpenseDetailsViewModel>();

            // views/pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AddLoadPage>();
            builder.Services.AddTransient<LoadDetailsPage>();
            builder.Services.AddTransient<AddExpensePage>();
            builder.Services.AddTransient<ExpenseDetailsPage>();

            // popups
            builder.Services.AddTransient<ZoomedImagePopup>();

            return builder.Build();
        }
    }
}
