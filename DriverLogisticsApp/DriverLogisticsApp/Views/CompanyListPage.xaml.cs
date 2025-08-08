using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class CompanyListPage : ContentPage
{
    private readonly CompanyListViewModel _viewModel;
    public CompanyListPage(CompanyListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.GetCompaniesCommand.ExecuteAsync(null);
    }
}