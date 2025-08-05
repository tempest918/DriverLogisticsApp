using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;

        /// <summary>
        /// initialize the main page with the view model
        /// </summary>
        /// <param name="viewModel"></param>
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        /// <summary>
        /// invoke viewmodel command to get loads when the page appears
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.GetLoadsCommand.ExecuteAsync(null);
        }
    }
}