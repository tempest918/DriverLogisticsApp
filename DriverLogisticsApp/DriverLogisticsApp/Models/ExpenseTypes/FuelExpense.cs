namespace DriverLogisticsApp.Models.ExpenseTypes
{
    public class FuelExpense : Expense
    {
        public override string FormattedDetails => $"Fuel Purchase on {Date:d}";
    }
}