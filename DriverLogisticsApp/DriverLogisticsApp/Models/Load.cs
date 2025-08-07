using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DriverLogisticsApp.Models
{
    public partial class Load : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string LoadNumber { get; set; } = string.Empty;

        // shipper
        public string ShipperName { get; set; } = string.Empty;
        public string ShipperAddressLineOne { get; set; } = string.Empty;
        public string? ShipperAddressLineTwo { get; set; }
        public string ShipperCity { get; set; } = string.Empty;
        public string ShipperState { get; set; } = string.Empty;
        public string ShipperZipCode { get; set; } = string.Empty;
        public string ShipperCountry { get; set; } = string.Empty;
        public string? ShipperPhoneNumber { get; set; }

        [Ignore]
        public List<string> ShipperAddressLines =>
            new List<string>
            {
            ShipperAddressLineOne,
            ShipperAddressLineTwo,
            $"{ShipperCity}, {ShipperState} {ShipperZipCode}",
            ShipperCountry,
            ShipperPhoneNumber
            }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();


        // consignee
        public string? ConsigneeName { get; set; }
        public string? ConsigneeAddressLineOne { get; set; }
        public string? ConsigneeAddressLineTwo { get; set; }
        public string? ConsigneeCity { get; set; }
        public string? ConsigneeState { get; set; }
        public string? ConsigneeZipCode { get; set; }
        public string? ConsigneeCountry { get; set; }
        public string? ConsigneePhoneNumber { get; set; }

        [Ignore]
        public List<string> ConsigneeAddressLines =>
            new List<string>
            {
            ConsigneeAddressLineOne,
            ConsigneeAddressLineTwo,
            $"{ConsigneeCity}, {ConsigneeState} {ConsigneeZipCode}",
            ConsigneeCountry,
            ConsigneePhoneNumber
            }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();


        // load details
        public DateTime PickupDate { get; set; }
        public DateTime? ActualPickupTime { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public decimal FreightRate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
