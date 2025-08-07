using SQLite;

namespace DriverLogisticsApp.Models
{
    public class UserProfile
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string? UserPhoneNumber { get; set; }

        public string? UserEmail { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string CompanyAddressLineOne { get; set; } = string.Empty;

        public string? CompanyAddressLineTwo { get; set; }

        public string CompanyCity { get; set; } = string.Empty;

        public string CompanyState { get; set; } = string.Empty;

        public string CompanyZipCode { get; set; } = string.Empty;

        public string CompanyCountry { get; set; } = string.Empty;

        public string? CompanyPhoneNumber { get; set; }
    }
}
