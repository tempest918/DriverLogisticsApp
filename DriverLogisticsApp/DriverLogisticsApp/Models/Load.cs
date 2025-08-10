using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DriverLogisticsApp.Models
{
    public partial class Load : ObservableObject
    {
        private int _id;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        [ObservableProperty]
        private string _loadNumber = string.Empty;

        [ObservableProperty]
        private int _shipperId;

        [ObservableProperty]
        private int? _consigneeId;

        [Ignore]
        public string ShipperName { get; set; } = string.Empty;
        [Ignore]
        public string ShipperAddress { get; set; } = string.Empty;
        [Ignore]
        public string? ConsigneeName { get; set; }
        [Ignore]
        public string? ConsigneeAddress { get; set; }

        [ObservableProperty]
        private DateTime _pickupDate;

        [ObservableProperty]
        private DateTime? _actualPickupTime;

        [ObservableProperty]
        private DateTime _deliveryDate;

        [ObservableProperty]
        private DateTime? _actualDeliveryTime;

        [ObservableProperty]
        private decimal _freightRate;

        [ObservableProperty]
        private string _status = string.Empty;

        [ObservableProperty]
        private bool _isCancelled = false;
    }
}
