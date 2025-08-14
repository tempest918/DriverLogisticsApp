using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class AllExpensesPage : ContentPage
{
	private readonly AllExpensesViewModel _viewModel;
	public AllExpensesPage(AllExpensesViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadExpensesCommand.ExecuteAsync(null);
    }
}
