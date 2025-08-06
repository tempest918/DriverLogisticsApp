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
        }
    }
}
