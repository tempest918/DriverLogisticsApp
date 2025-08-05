using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class AddLoadPage : ContentPage
{
    /// <summary>
    /// initialize the add load page with the add load view model
    /// </summary>
    /// <param name="viewModel"></param>
    public AddLoadPage(AddLoadViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}