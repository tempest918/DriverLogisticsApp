using DriverLogisticsApp.Models;
using DriverLogisticsApp.Models.ExpenseTypes;
using SQLite;

namespace DriverLogisticsApp.Services
{
    public class DatabaseService : IDatabaseService
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

            // create the tables
            await _database.CreateTableAsync<Load>();
            await _database.CreateTableAsync<Company>();
            await _database.CreateTableAsync<Models.Expense>();
            await _database.CreateTableAsync<UserProfile>();
        }

        #region Load CRUD operations
        /// <summary>
        /// retrieve all loads from the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<Load>> GetLoadsAsync()
        {
            await Init();
            var loads = await _database!.Table<Load>().ToListAsync();
            var companies = await GetCompaniesAsync();

            // "Join" the company data to each load
            foreach (var load in loads)
            {
                var shipper = companies.FirstOrDefault(c => c.Id == load.ShipperId);
                if (shipper != null)
                {
                    load.ShipperName = shipper.Name;
                    load.ShipperAddress = FormatAddress(shipper);
                }

                if (load.ConsigneeId.HasValue)
                {
                    var consignee = companies.FirstOrDefault(c => c.Id == load.ConsigneeId.Value);
                    if (consignee != null)
                    {
                        load.ConsigneeName = consignee.Name;
                        load.ConsigneeAddress = FormatAddress(consignee);
                    }
                }
            }
            return loads;
        }

        /// <summary>
        /// get a single load by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Load> GetLoadAsync(int id)
        {
            await Init();
            var load = await _database!.Table<Load>().Where(l => l.Id == id).FirstOrDefaultAsync();
            if (load != null)
            {
                var shipper = await GetCompanyAsync(load.ShipperId);
                if (shipper != null)
                {
                    load.ShipperName = shipper.Name;
                    load.ShipperAddress = FormatAddress(shipper);
                }

                if (load.ConsigneeId.HasValue)
                {
                    var consignee = await GetCompanyAsync(load.ConsigneeId.Value);
                    if (consignee != null)
                    {
                        load.ConsigneeName = consignee.Name;
                        load.ConsigneeAddress = FormatAddress(consignee);
                    }
                }
            }
            return load;
        }

        /// <summary>
        /// helper to format a company address
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        private string FormatAddress(Company company)
        {
            var addressParts = new List<string>
            {
                company.AddressLineOne,
                company.AddressLineTwo,
                $"{company.City}, {company.State} {company.ZipCode}",
                company.Country
            };
            return string.Join("\n", addressParts.Where(s => !string.IsNullOrWhiteSpace(s)));
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
        /// rather than deleting a load, we mark it as cancelled
        /// </summary>
        /// <param name="load"></param>
        /// <returns></returns>
        public async Task<int> DeleteLoadAsync(Load load)
        {
            await Init();
            load.Status = "Cancelled";
            load.IsCancelled = true;

            return await SaveLoadAsync(load);
        }
        #endregion

        #region Company CRUD operations

        /// <summary>
        /// get all companies (shippers and consignees)
        /// </summary>
        /// <returns></returns>
        public async Task<List<Company>> GetCompaniesAsync()
        {
            await Init();
            return await _database!.Table<Company>().ToListAsync();
        }

        /// <summary>
        /// get a single company by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Company> GetCompanyAsync(int id)
        {
            await Init();
            return await _database!.Table<Company>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// save a company (insert or update)
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<int> SaveCompanyAsync(Company company)
        {
            await Init();
            if (company.Id != 0)
            {
                return await _database!.UpdateAsync(company);
            }
            else
            {
                return await _database!.InsertAsync(company);
            }
        }

        /// <summary>
        /// delete a company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<int> DeleteCompanyAsync(Company company)
        {
            await Init();
            return await _database!.DeleteAsync(company);
        }
        #endregion

        #region Expense CRUD operations

        /// <summary>
        /// get a simple expense by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Models.Expense> GetSimpleExpenseAsync(int id)
        {
            await Init();
            return await _database!.Table<Models.Expense>().Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// get all expenses for a specific load, utilize factory pattern to convert for polymorphic behavior
        /// </summary>
        /// <param name="loadId"></param>
        /// <returns></returns>
        public async Task<List<Models.ExpenseTypes.Expense>> GetExpensesForLoadAsync(int loadId)
        {
            await Init();
            List<Models.Expense> dbExpenses;

            if (loadId != 0)
            {
                // get all expenses for the specified load
                dbExpenses = await _database!.Table<Models.Expense>().Where(e => e.LoadId == loadId).ToListAsync();
            }
            else
            {
                // get all expenses for all loads if loadId is 0
                dbExpenses = await _database!.Table<Models.Expense>().ToListAsync();
            }

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
                        specificExpense = new GeneralExpense();
                        break;
                }

                // populate the specific expense properties
                specificExpense.Id = dbExpense.Id;
                specificExpense.LoadId = dbExpense.LoadId;
                specificExpense.Amount = dbExpense.Amount;
                specificExpense.Date = dbExpense.Date;
                specificExpense.Description = dbExpense.Description;
                specificExpense.ReceiptImagePath = dbExpense.ReceiptImagePath;
                specificExpense.Category = dbExpense.Category;

                expenseList.Add(specificExpense);
            }

            return expenseList;
        }

        /// <summary>
        /// get a single expense by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Models.ExpenseTypes.Expense?> GetExpenseAsync(int id)
        {
            await Init();

            // get single expense from the database
            var dbExpense = await _database!.Table<Models.Expense>().Where(e => e.Id == id).FirstOrDefaultAsync();

            if (dbExpense is null)
                return null;

            // convert to the specific type of expense based on the category
            Models.ExpenseTypes.Expense specificExpense;
            switch (dbExpense.Category)
            {
                case "Fuel":
                    specificExpense = new Models.ExpenseTypes.FuelExpense();
                    break;
                case "Maintenance":
                    specificExpense = new Models.ExpenseTypes.MaintenanceExpense();
                    break;
                default:
                    specificExpense = new Models.ExpenseTypes.MaintenanceExpense { Description = dbExpense.Category };
                    break;
            }

            // populate the specific expense properties
            specificExpense.Id = dbExpense.Id;
            specificExpense.LoadId = dbExpense.LoadId;
            specificExpense.Amount = dbExpense.Amount;
            specificExpense.Date = dbExpense.Date;
            if (string.IsNullOrEmpty(specificExpense.Description))
            {
                specificExpense.Description = dbExpense.Description;
            }
            specificExpense.ReceiptImagePath = dbExpense.ReceiptImagePath;

            return specificExpense;
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

        #region UserProfile operations

        /// <summary>
        /// get the user profile, there will only ever be one
        /// </summary>
        /// <returns></returns>
        public async Task<UserProfile> GetUserProfileAsync()
        {
            await Init();
            var profile = await _database!.Table<UserProfile>().FirstOrDefaultAsync();

            if (profile == null)
            {
                profile = new UserProfile { Id = 1 };
                await _database.InsertAsync(profile);
            }

            return profile;
        }

        /// <summary>
        /// save the user profile, there will only ever be one with Id = 1
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public async Task<int> SaveUserProfileAsync(UserProfile profile)
        {
            await Init();

            profile.Id = 1;

            return await _database!.InsertOrReplaceAsync(profile);
        }
        #endregion

    }
}