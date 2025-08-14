using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class LoadsArchivePage : ContentPage
{
    private readonly LoadsArchiveViewModel _viewModel;
    public LoadsArchivePage(LoadsArchiveViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadLoadsCommand.ExecuteAsync(null);
    }
}
