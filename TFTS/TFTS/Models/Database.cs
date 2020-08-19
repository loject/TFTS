using SQLite;
using System.Collections.Generic;
using System.Linq;
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

        public List<RaceModel> GetRaceHistory()
        {
            var tmp = _database.Table<RaceModel>().ToListAsync();
            tmp.Wait();
            return tmp.Result;
        }

        public int SaveRaceToRaceHistory(RaceModel data)
        {
            var tmp = _database.InsertAsync(data);
            tmp.Wait();
            return tmp.Result;
        }
    }
}
