using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp;

public partial class AppShell : Shell
{
	public AppShell(AppShellViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

		Routing.RegisterRoute(nameof(Views.AddLoadPage), typeof(Views.AddLoadPage));
        Routing.RegisterRoute(nameof(Views.LoadDetailsPage), typeof(Views.LoadDetailsPage));
        Routing.RegisterRoute(nameof(Views.AddExpensePage), typeof(Views.AddExpensePage));
        Routing.RegisterRoute(nameof(Views.ExpenseDetailsPage), typeof(Views.ExpenseDetailsPage));
        Routing.RegisterRoute(nameof(Views.SettlementReportPage), typeof(Views.SettlementReportPage));
        Routing.RegisterRoute(nameof(Views.ProfilePage), typeof(Views.ProfilePage));
        Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
        Routing.RegisterRoute(nameof(Views.CompanyListPage), typeof(Views.CompanyListPage));
        Routing.RegisterRoute(nameof(Views.AddEditCompanyPage), typeof(Views.AddEditCompanyPage));
        Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
    }
}
