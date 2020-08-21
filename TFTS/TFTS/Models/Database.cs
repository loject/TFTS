using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TFTS.Models
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<RaceModel>().Wait();
        }

        public Task<List<RaceModel>> GetRaceHistory() => _database.Table<RaceModel>().ToListAsync();
        public Task<int> SaveRaceToRaceHistory(RaceModel data) => _database.InsertAsync(data);
        public Task<int> ClearHistory() => _database.Table<RaceModel>().DeleteAsync( r => true );
    }
}
