using DriverLogisticsApp.Models;
using ExpenseBase = DriverLogisticsApp.Models.ExpenseTypes.Expense;

namespace DriverLogisticsApp.Services
{
    public interface IDatabaseService
    {
        Task<List<Load>> GetLoadsAsync();

        Task<Load> GetLoadAsync(int id);

        Task<int> SaveLoadAsync(Load load);

        Task<int> DeleteLoadAsync(Load load);

        Task<List<ExpenseBase>> GetExpensesForLoadAsync(int loadId);

        Task<ExpenseBase?> GetExpenseAsync(int id);

        Task<int> SaveExpenseAsync(Expense expense);

        Task<int> DeleteExpenseAsync(Expense expense);

        Task<Expense> GetSimpleExpenseAsync(int id);
    }
}