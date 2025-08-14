using DriverLogisticsApp.Views;
using System.ComponentModel;

namespace DriverLogisticsApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            this.PropertyChanged += AppShell_PropertyChanged;

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
            Routing.RegisterRoute(nameof(AllExpensesPage), typeof(AllExpensesPage));
            Routing.RegisterRoute(nameof(LoadsArchivePage), typeof(LoadsArchivePage));
        }

        private async void AppShell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentItem))
            {
                if (Shell.Current.Navigation.NavigationStack.Count > 1)
                {
                    await Shell.Current.Navigation.PopToRootAsync();
                }
            }
        }
    }
}
