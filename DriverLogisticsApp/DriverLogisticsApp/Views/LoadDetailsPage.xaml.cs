using DriverLogisticsApp.ViewModels;
using System.ComponentModel;

namespace DriverLogisticsApp.Views;

public partial class LoadDetailsPage : ContentPage
{
    private readonly LoadDetailsViewModel _viewModel;

    /// <summary>
    /// load details page using the LoadDetailsViewModel
    /// </summary>
    /// <param name="viewModel"></param>
	public LoadDetailsPage(LoadDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// refresh load details when page loads
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();

        // update the toolbar based on the view model state
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        UpdateToolbar();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
    }

    /// <summary>
    /// monitor property changes in the view model to update the toolbar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoadDetailsViewModel.Load))
        {
            UpdateToolbar();
        }
    }

    /// <summary>
    /// used to update the toolbar based on the view model state
    /// </summary>
    private void UpdateToolbar()
    {
        // This ensures the UI update happens on the correct thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Clear any existing buttons to prevent duplicates
            this.ToolbarItems.Clear();

            // If the ViewModel says the button should be visible, create and add it
            if (_viewModel.IsToolbarActionVisible)
            {
                var toolbarItem = new ToolbarItem
                {
                    Text = _viewModel.ToolbarActionText,
                    Command = _viewModel.ToolbarActionCommand
                };
                this.ToolbarItems.Add(toolbarItem);
            }
        });
    }
}