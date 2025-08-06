using DriverLogisticsApp.Models;
using DriverLogisticsApp.Models.ExpenseTypes;
using SQLite;

namespace DriverLogisticsApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        /// <summary>
        /// set up the database
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            if (_database is not null)
                return;

            // get path to the database file
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "DriverLogistics.db3");

            _database = new SQLiteAsyncConnection(databasePath);

            // create the Load table if it doesn't exist
            await _database.CreateTableAsync<Load>();
            await _database.CreateTableAsync<Models.Expense>();
        }

        #region Load CRUD operations
        /// <summary>
        /// retrieve all loads from the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<Load>> GetLoadsAsync()
        {
            await Init();
            return await _database.Table<Load>().ToListAsync();
        }

        /// <summary>
        /// get a single load by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Load> GetLoadAsync(int id)
        {
            await Init();
            return await _database.Table<Load>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// save a load (insert or update)
        /// </summary>
        /// <param name="load"></param>
        /// <returns></returns>
        public async Task<int> SaveLoadAsync(Load load)
        {
            await Init();
            if (load.Id != 0)
            {
                // update an existing load
                return await _database.UpdateAsync(load);
            }
            else
            {
                // save a new load
                return await _database.InsertAsync(load);
            }
        }

        /// <summary>
        /// delete a load
        /// </summary>
        /// <param name="load"></param>
        /// <returns></returns>
        public async Task<int> DeleteLoadAsync(Load load)
        {
            await Init();
            return await _database.DeleteAsync(load);
        }
        #endregion

        #region Expense CRUD operations

        /// <summary>
        /// get all expenses for a specific load, utilize factory pattern to convert for polymorphic behavior
        /// </summary>
        /// <param name="loadId"></param>
        /// <returns></returns>
        public async Task<List<Models.ExpenseTypes.Expense>> GetExpensesForLoadAsync(int loadId)
        {
            await Init();

            // get all expenses for the specified load
            var dbExpenses = await _database!.Table<Models.Expense>().Where(e => e.LoadId == loadId).ToListAsync();

            var expenseList = new List<Models.ExpenseTypes.Expense>();

            // for each database expense, create the specific type of Expense
            foreach (var dbExpense in dbExpenses)
            {
                Models.ExpenseTypes.Expense specificExpense;
                switch (dbExpense.Category)
                {
                    case "Fuel":
                        specificExpense = new FuelExpense();
                        break;
                    case "Maintenance":
                        specificExpense = new MaintenanceExpense();
                        break;
                    default:
                        specificExpense = new MaintenanceExpense { Description = dbExpense.Category }; // Fallback
                        break;
                }

                // populate the specific expense properties
                specificExpense.Id = dbExpense.Id;
                specificExpense.LoadId = dbExpense.LoadId;
                specificExpense.Amount = dbExpense.Amount;
                specificExpense.Date = dbExpense.Date;
                specificExpense.Description = dbExpense.Description;
                specificExpense.ReceiptImagePath = dbExpense.ReceiptImagePath;

                expenseList.Add(specificExpense);
            }

            return expenseList;
        }

        /// <summary>
        /// insert or update an expense
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        public async Task<int> SaveExpenseAsync(Models.Expense expense)
        {
            await Init();
            if (expense.Id != 0)
            {
                return await _database!.UpdateAsync(expense);
            }
            else
            {
                return await _database!.InsertAsync(expense);
            }
        }

        /// <summary>
        /// delete an expense
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        public async Task<int> DeleteExpenseAsync(Models.Expense expense)
        {
            await Init();
            return await _database!.DeleteAsync(expense);
        }
        #endregion
    }
}