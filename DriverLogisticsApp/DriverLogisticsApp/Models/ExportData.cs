using DriverLogisticsApp.Models;

namespace DriverLogisticsApp.Models
{
    public class ExportData
    {
        public List<Load> Loads { get; set; } = new List<Load>();
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        public List<Company> Companies { get; set; } = new List<Company>();
        public UserProfile UserProfile { get; set; }
    }
}