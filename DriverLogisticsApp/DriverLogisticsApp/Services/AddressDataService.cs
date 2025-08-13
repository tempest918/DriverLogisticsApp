using System.Collections.Generic;
using System.Linq;

namespace DriverLogisticsApp.Services
{
    public class AddressDataService
    {
        private readonly Dictionary<string, string> _usStates;
        private readonly Dictionary<string, string> _canadianProvinces;
        private readonly Dictionary<string, string> _mexicanStates;

        public AddressDataService()
        {
            _usStates = GetUSStates();
            _canadianProvinces = GetCanadianProvinces();
            _mexicanStates = GetMexicanStates();
        }

        public List<string> GetCountries()
        {
            return new List<string> { "USA", "Canada", "Mexico" };
        }

        public List<string> GetStatesProvincesForCountry(string country)
        {
            return country switch
            {
                "USA" => _usStates.Keys.ToList(),
                "Canada" => _canadianProvinces.Keys.ToList(),
                "Mexico" => _mexicanStates.Keys.ToList(),
                _ => new List<string>()
            };
        }

        public string GetStateAbbreviation(string fullStateName)
        {
            var allStates = _usStates.Concat(_canadianProvinces).Concat(_mexicanStates);
            var entry = allStates.FirstOrDefault(kvp => kvp.Value.Equals(fullStateName, System.StringComparison.OrdinalIgnoreCase));
            return entry.Key; // Returns null if not found
        }


        private Dictionary<string, string> GetUSStates()
        {
            return new Dictionary<string, string>
            {
                { "AL", "Alabama" }, { "AK", "Alaska" }, { "AZ", "Arizona" }, { "AR", "Arkansas" }, { "CA", "California" },
                { "CO", "Colorado" }, { "CT", "Connecticut" }, { "DE", "Delaware" }, { "FL", "Florida" }, { "GA", "Georgia" },
                { "HI", "Hawaii" }, { "ID", "Idaho" }, { "IL", "Illinois" }, { "IN", "Indiana" }, { "IA", "Iowa" },
                { "KS", "Kansas" }, { "KY", "Kentucky" }, { "LA", "Louisiana" }, { "ME", "Maine" }, { "MD", "Maryland" },
                { "MA", "Massachusetts" }, { "MI", "Michigan" }, { "MN", "Minnesota" }, { "MS", "Mississippi" }, { "MO", "Missouri" },
                { "MT", "Montana" }, { "NE", "Nebraska" }, { "NV", "Nevada" }, { "NH", "New Hampshire" }, { "NJ", "New Jersey" },
                { "NM", "New Mexico" }, { "NY", "New York" }, { "NC", "North Carolina" }, { "ND", "North Dakota" }, { "OH", "Ohio" },
                { "OK", "Oklahoma" }, { "OR", "Oregon" }, { "PA", "Pennsylvania" }, { "RI", "Rhode Island" }, { "SC", "South Carolina" },
                { "SD", "South Dakota" }, { "TN", "Tennessee" }, { "TX", "Texas" }, { "UT", "Utah" }, { "VT", "Vermont" },
                { "VA", "Virginia" }, { "WA", "Washington" }, { "WV", "West Virginia" }, { "WI", "Wisconsin" }, { "WY", "Wyoming" }
            };
        }

        private Dictionary<string, string> GetCanadianProvinces()
        {
            return new Dictionary<string, string>
            {
                { "AB", "Alberta" }, { "BC", "British Columbia" }, { "MB", "Manitoba" }, { "NB", "New Brunswick" },
                { "NL", "Newfoundland and Labrador" }, { "NS", "Nova Scotia" }, { "NT", "Northwest Territories" },
                { "NU", "Nunavut" }, { "ON", "Ontario" }, { "PE", "Prince Edward Island" }, { "QC", "Quebec" },
                { "SK", "Saskatchewan" }, { "YT", "Yukon" }
            };
        }

        private Dictionary<string, string> GetMexicanStates()
        {
            return new Dictionary<string, string>
            {
                { "AGS", "Aguascalientes" }, { "BC", "Baja California" }, { "BCS", "Baja California Sur" },
                { "CAMP", "Campeche" }, { "CHIH", "Chihuahua" }, { "CHIS", "Chiapas" }, { "CMX", "Mexico City" },
                { "COAH", "Coahuila" }, { "COL", "Colima" }, { "DGO", "Durango" }, { "GRO", "Guerrero" },
                { "GTO", "Guanajuato" }, { "HGO", "Hidalgo" }, { "JAL", "Jalisco" }, { "MEX", "Mexico" },
                { "MICH", "Michoacán" }, { "MOR", "Morelos" }, { "NAY", "Nayarit" }, { "NL", "Nuevo León" },
                { "OAX", "Oaxaca" }, { "PUE", "Puebla" }, { "QRO", "Querétaro" }, { "QROO", "Quintana Roo" },
                { "SLP", "San Luis Potosí" }, { "SIN", "Sinaloa" }, { "SON", "Sonora" }, { "TAB", "Tabasco" },
                { "TAMS", "Tamaulipas" }, { "TLAX", "Tlaxcala" }, { "VER", "Veracruz" }, { "YUC", "Yucatán" },
                { "ZAC", "Zacatecas" }
            };
        }
    }
}
