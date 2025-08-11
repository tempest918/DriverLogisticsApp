using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DriverLogisticsApp.Models
{
    public partial class UserProfile : ObservableObject
    {
        private int _id = 1;

        [PrimaryKey]
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string? _userPhoneNumber;

        [ObservableProperty]
        private string? _userEmail;

        [ObservableProperty]
        private string _companyName = string.Empty;

        [ObservableProperty]
        private string _companyAddressLineOne = string.Empty;

        [ObservableProperty]
        private string? _companyAddressLineTwo;

        [ObservableProperty]
        private string _companyCity = string.Empty;

        [ObservableProperty]
        private string _companyState = string.Empty;

        [ObservableProperty]
        private string _companyZipCode = string.Empty;

        [ObservableProperty]
        private string _companyCountry = string.Empty;

        [ObservableProperty]
        private string? _companyPhoneNumber;
    }
}
