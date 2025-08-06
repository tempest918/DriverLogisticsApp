using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class AddExpensePage : ContentPage
{
    private readonly AddExpenseViewModel _viewModel;

    /// <summary>
    /// initialize the AddExpensePage with the provided view model
    /// </summary>
    /// <param name="viewModel"></param>
    public AddExpensePage(AddExpenseViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <summary>
    /// override OnAppearing to load the expense data for editing
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadExpenseForEditAsync();
    }
}