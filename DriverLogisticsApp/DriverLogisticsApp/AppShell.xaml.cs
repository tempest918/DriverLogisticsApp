using DriverLogisticsApp.Views;

namespace DriverLogisticsApp
{
    public partial class AppShell : Shell
    {
        private bool _isTabChange;
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
            Routing.RegisterRoute(nameof(AllExpensesPage), typeof(AllExpensesPage));
            Routing.RegisterRoute(nameof(LoadsArchivePage), typeof(LoadsArchivePage));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            _isTabChange = args.Source == ShellNavigationSource.ShellSectionChanged;
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);

            if (_isTabChange)
            {
                if (Shell.Current.Navigation.NavigationStack.Count > 1)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.Navigation.PopToRootAsync(false);
                    });
                }
                _isTabChange = false;
            }
        }
    }
}
