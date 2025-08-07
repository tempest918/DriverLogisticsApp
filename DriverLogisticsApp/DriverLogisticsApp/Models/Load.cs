using SQLite;

namespace DriverLogisticsApp.Models
{
    public class Load
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string LoadNumber { get; set; } = string.Empty;

        public string ShipperName { get; set; } = string.Empty;

        public string ConsigneeName { get; set; } = string.Empty;

        public DateTime PickupDate { get; set; }

        public DateTime? ActualPickupTime { get; set; }

        public DateTime DeliveryDate { get; set; }

        public DateTime? ActualDeliveryTime { get; set; }

        public decimal FreightRate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
