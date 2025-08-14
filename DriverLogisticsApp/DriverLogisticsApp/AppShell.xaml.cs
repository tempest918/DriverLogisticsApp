using DriverLogisticsApp.Views;

namespace DriverLogisticsApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // routing for navigation
            Routing.RegisterRoute("AddLoadPage", typeof(Views.AddLoadPage));
            Routing.RegisterRoute("LoadDetailsPage", typeof(Views.LoadDetailsPage));
            Routing.RegisterRoute("AddExpensePage", typeof(Views.AddExpensePage));
            Routing.RegisterRoute("ExpenseDetailsPage", typeof(Views.ExpenseDetailsPage));
            Routing.RegisterRoute("SettlementReportPage", typeof(Views.SettlementReportPage));
            Routing.RegisterRoute("ProfilePage", typeof(Views.ProfilePage));
            Routing.RegisterRoute("LoginPage", typeof(Views.LoginPage));
            Routing.RegisterRoute("CompanyListPage", typeof(Views.CompanyListPage));
            Routing.RegisterRoute("AddEditCompanyPage", typeof(Views.AddEditCompanyPage));
            Routing.RegisterRoute(nameof(PrivacyPolicyPage), typeof(PrivacyPolicyPage));
        }
    }
}
