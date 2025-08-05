using DriverLogisticsApp.Models;
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
        }

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
    }
}