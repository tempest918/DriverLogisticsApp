using Microsoft.Extensions.Logging;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using DriverLogisticsApp.Views;
using DriverLogisticsApp.Popups;
using DriverLogisticsApp;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using IPreferences = DriverLogisticsApp.Services.IPreferences;

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
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialSymbolsOutlined-Regular.ttf", "MaterialIcons");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // services
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<IAlertService, MauiAlertService>();
            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            builder.Services.AddSingleton<ISecureStorageService, MauiSecureStorageService>();
            builder.Services.AddSingleton<IJsonImportExportService, JsonImportExportService>();
            builder.Services.AddSingleton<PdfService>();
            builder.Services.AddSingleton<AddressDataService>();
            builder.Services.AddSingleton<IPreferences, PreferencesService>();

            // view models
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<AddLoadViewModel>();
            builder.Services.AddTransient<LoadDetailsViewModel>();
            builder.Services.AddTransient<AddExpenseViewModel>();
            builder.Services.AddTransient<ExpenseDetailsViewModel>();
            builder.Services.AddTransient<SettlementReportViewModel>();
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<CompanyListViewModel>();
            builder.Services.AddTransient<AddEditCompanyViewModel>();
            builder.Services.AddTransient<SettingsPageViewModel>();

            // views/pages
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AddLoadPage>();
            builder.Services.AddTransient<LoadDetailsPage>();
            builder.Services.AddTransient<AddExpensePage>();
            builder.Services.AddTransient<ExpenseDetailsPage>();
            builder.Services.AddTransient<SettlementReportPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<CompanyListPage>();
            builder.Services.AddTransient<AddEditCompanyPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<PrivacyPolicyPage>();


            // popups
            builder.Services.AddTransient<ZoomedImagePopup>();

            var app = builder.Build();

            ServiceHelper.Initialize(app.Services);

            return app;
        }
    }
}
