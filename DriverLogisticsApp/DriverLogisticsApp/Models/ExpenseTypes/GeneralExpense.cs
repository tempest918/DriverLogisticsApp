namespace DriverLogisticsApp.Models.ExpenseTypes
{
    public class GeneralExpense : Expense
    {
        public override string FormattedDetails => $"{Category} on {Date:d}";
    }
}