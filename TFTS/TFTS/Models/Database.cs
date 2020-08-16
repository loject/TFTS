using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TFTS.Models
{
    public class DBRunner
    {
        public string Name { get; set; } = "Runner";
    }
    public class DBRace
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public List<DBRunner> Runners { get; set; }

        public static DBRace CreateFromModel(RaceModel race)
        {
            DBRace res = new DBRace();
            return res;
        }
    }
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<DBRace>().Wait();
        }

        public Task<List<DBRace>> GetPeopleAsync()
        {
            return _database.Table<DBRace>().ToListAsync();
        }

        public Task<int> SavePersonAsync(DBRace data)
        {
            return _database.InsertAsync(data);
        }
    }
}
