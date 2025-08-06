using DriverLogisticsApp.Models.ExpenseTypes;
using System.Collections.Generic;
using ExpenseBase = DriverLogisticsApp.Models.ExpenseTypes.Expense;

namespace DriverLogisticsApp.Models
{
    // a class to group expenses by load number and total amount for that load
    public class LoadExpenseGroup : List<ExpenseBase>
    {
        public string LoadNumber { get; private set; }
        public decimal TotalForLoad { get; private set; }

        public LoadExpenseGroup(string loadNumber, decimal totalForLoad, List<ExpenseBase> expenses) : base(expenses)
        {
            LoadNumber = loadNumber;
            TotalForLoad = totalForLoad;
        }
    }
}