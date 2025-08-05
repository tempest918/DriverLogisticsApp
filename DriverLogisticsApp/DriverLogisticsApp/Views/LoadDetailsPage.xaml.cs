using DriverLogisticsApp.ViewModels;

namespace DriverLogisticsApp.Views;

public partial class LoadDetailsPage : ContentPage
{
    /// <summary>
    /// load details page using the LoadDetailsViewModel
    /// </summary>
    /// <param name="viewModel"></param>
    public LoadDetailsPage(LoadDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}