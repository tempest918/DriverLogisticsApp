using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class AddEditCompanyPage : ContentPage
{
    public AddEditCompanyPage(AddEditCompanyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}