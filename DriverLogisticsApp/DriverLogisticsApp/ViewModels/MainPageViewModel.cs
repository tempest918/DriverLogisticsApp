using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

namespace DriverLogisticsApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Load> _loads;

        /// <summary>
        /// initialize the view model for the main page
        /// </summary>
        /// <param name="databaseService"></param>
        public MainPageViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Loads = new ObservableCollection<Load>();
        }

        /// <summary>
        /// load all loads from the database
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GetLoadsAsync()
        {
            var loadsFromDb = await _databaseService.GetLoadsAsync();

            // clear the existing loads and add the new ones
            Loads.Clear();
            foreach (var load in loadsFromDb)
            {
                Loads.Add(load);
            }
        }

        /// <summary>
        /// navigate to add load page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToAddLoadAsync()
        {
            await Shell.Current.GoToAsync("AddLoadPage");
        }
    }
}