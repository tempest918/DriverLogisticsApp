using SQLite;

namespace DriverLogisticsApp.Models
{
    public class Expense
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int LoadId { get; set; }

        public string Category { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string? Description { get; set; }

        public string? ReceiptImagePath { get; set; }
    }
}