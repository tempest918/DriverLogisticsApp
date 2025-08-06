namespace DriverLogisticsApp.Models.ExpenseTypes
{
    public abstract class Expense
    {
        public int Id { get; set; }

        public int LoadId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string? Description { get; set; }

        public string? ReceiptImagePath { get; set; }

        public string Category { get; set; } = string.Empty;

        public abstract string FormattedDetails { get; }
    }
}