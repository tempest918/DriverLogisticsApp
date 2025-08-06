using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class ExpenseDetailsPage : ContentPage
{
    /// <summary>
    /// load details page using the ExpenseDetailsViewModel
    /// </summary>
    /// <param name="viewModel"></param>
    public ExpenseDetailsPage(ExpenseDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}