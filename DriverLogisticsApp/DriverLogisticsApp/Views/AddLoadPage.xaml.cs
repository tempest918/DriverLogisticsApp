using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class AddLoadPage : ContentPage
{
    private readonly AddLoadViewModel _viewModel;

    /// <summary>
    /// initialize the add load page with the add load view model
    /// </summary>
    /// <param name="viewModel"></param>
    public AddLoadPage(AddLoadViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// add load page appears, load data for editing if necessary
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}