using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        /// <summary>
        /// check if the PIN entry has 4 characters, and if so, execute the login command
        /// </summary>
        private void PinEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BindingContext is LoginViewModel viewModel && e.NewTextValue?.Length == 4)
            {
                if (viewModel.LoginCommand.CanExecute(null))
                {
                    viewModel.LoginCommand.Execute(null);
                }
            }
        }
    }
}
