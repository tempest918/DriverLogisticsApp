namespace DriverLogisticsApp.Models.ExpenseTypes
{
    public class MaintenanceExpense : Expense
    {
        public override string FormattedDetails => $"Maintenance on {Date:d}";
    }
}