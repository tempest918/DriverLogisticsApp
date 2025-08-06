using DriverLogisticsApp.ViewModels;
namespace DriverLogisticsApp.Views;

public partial class SettlementReportPage : ContentPage
{
    public SettlementReportPage(SettlementReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}