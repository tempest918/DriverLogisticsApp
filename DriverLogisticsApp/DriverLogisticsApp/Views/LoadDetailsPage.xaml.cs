using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class LoadDetailsPage : ContentPage
{
    private readonly LoadDetailsViewModel _viewModel;

    /// <summary>
    /// load details page using the LoadDetailsViewModel
    /// </summary>
    /// <param name="viewModel"></param>
	public LoadDetailsPage(LoadDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// refresh load details when page loads
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }
}