using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class AddExpensePage : ContentPage
{
    public AddExpensePage(AddExpenseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}