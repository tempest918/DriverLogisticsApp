using DriverLogisticsApp.Views;

namespace DriverLogisticsApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // check if a PIN is saved in secure storage
            var savedPin = SecureStorage.Default.GetAsync("user_pin").Result;

            if (string.IsNullOrWhiteSpace(savedPin))
            {
                // if no pin exists, show the main app shell.
                MainPage = ServiceHelper.Services.GetService<AppShell>();
            }
            else
            {
                // if a pin exists, show the login page.
                MainPage = ServiceHelper.Services.GetService<LoginPage>();
            }
        }
    }
}
