namespace DriverLogisticsApp.Views;
using DriverLogisticsApp.ViewModels;

public partial class ImportExportPage : ContentPage
{
	public ImportExportPage(ImportExportViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

    }
}