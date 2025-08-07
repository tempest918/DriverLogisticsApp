using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfilePageViewModel _viewModel;

    /// <summary>
    /// initializes the page with the provided view model
    /// </summary>
    /// <param name="viewModel"></param>
    public ProfilePage(ProfilePageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// upon appearing, load the user profile
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProfileAsync();
    }
}