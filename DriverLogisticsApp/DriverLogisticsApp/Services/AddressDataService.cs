using System.Collections.Generic;

namespace DriverLogisticsApp.Services
{
    public class AddressDataService
    {
        public List<string> GetCountries()
        {
            return new List<string> { "USA", "Canada", "Mexico" };
        }

        public List<string> GetStatesProvincesForCountry(string country)
        {
            return country switch
            {
                "USA" => GetUSStates(),
                "Canada" => GetCanadianProvinces(),
                "Mexico" => GetMexicanStates(),
                _ => new List<string>()
            };
        }

        private List<string> GetUSStates()
        {
            return new List<string> { "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA", "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ", "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY" };
        }

        private List<string> GetCanadianProvinces()
        {
            return new List<string> { "AB", "BC", "MB", "NB", "NL", "NS", "NT", "NU", "ON", "PE", "QC", "SK", "YT" };
        }

        private List<string> GetMexicanStates()
        {
            return new List<string> { "AGS", "BC", "BCS", "CAMP", "CHIH", "CHIS", "CMX", "COAH", "COL", "DGO", "GRO", "GTO", "HGO", "JAL", "MEX", "MICH", "MOR", "NAY", "NL", "OAX", "PUE", "QRO", "QROO", "SLP", "SIN", "SON", "TAB", "TAMS", "TLAX", "VER", "YUC", "ZAC" };
        }
    }
}
