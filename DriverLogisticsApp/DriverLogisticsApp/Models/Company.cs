using SQLite;

namespace DriverLogisticsApp.Models
{
    public class Company
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Name { get; set; } = string.Empty;

        public string AddressLineOne { get; set; } = string.Empty;

        public string? AddressLineTwo { get; set; }

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}
