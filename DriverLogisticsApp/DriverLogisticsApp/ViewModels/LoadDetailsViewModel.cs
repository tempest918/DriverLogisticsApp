using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Diagnostics;

namespace DriverLogisticsApp.ViewModels
{
    // pass LoadId as a query parameter to this view model
    [QueryProperty(nameof(LoadId), "LoadId")]

    public partial class LoadDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int _loadId;

        [ObservableProperty]
        private Load _load;

        /// <summary>
        /// initialize the view model for the load details page
        /// </summary>
        /// <param name="databaseService"></param>
        public LoadDetailsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// invoked when LoadId changes, loads data for the specified load
        /// </summary>
        /// <param name="value"></param>
        partial void OnLoadIdChanged(int value)
        {
            // Load the full details of the load from the database
            LoadDataAsync();
        }

        /// <summary>
        /// used by OnLoadIdChanged to load the full details of the load
        /// </summary>
        /// <returns></returns>
        private async Task LoadDataAsync()
        {
            try
            {
                var load = await _databaseService.GetLoadAsync(LoadId);
                if (load != null)
                {
                    Load = load;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load data: {ex.Message}");
            }
        }

        /// <summary>
        /// delete the current load
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task DeleteLoadAsync()
        {
            // ask for confirmation before deleting
            bool confirmed = await Shell.Current.DisplayAlert("Confirm Delete", $"Are you sure you want to delete Load #{Load.LoadNumber}?", "Yes", "No");
            if (!confirmed)
                return;

            await _databaseService.DeleteLoadAsync(Load);

            // navigate back to the main list
            await Shell.Current.GoToAsync("..");
        }
    }
}