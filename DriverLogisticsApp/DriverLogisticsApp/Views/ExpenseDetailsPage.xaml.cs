using CommunityToolkit.Maui.Extensions;
using DriverLogisticsApp.ViewModels;
using DriverLogisticsApp.Popups;

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

    /// <summary>
    /// on tap even for the image, show the zoomed image popup
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnImageTapped(object sender, TappedEventArgs e)
    {
        var viewModel = (ExpenseDetailsViewModel)BindingContext;

        if (string.IsNullOrWhiteSpace(viewModel.Expense?.ReceiptImagePath))
            return;

        await this.ShowPopupAsync(new ZoomedImagePopup(viewModel.Expense.ReceiptImagePath));
    }
}