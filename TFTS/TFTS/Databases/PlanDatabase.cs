using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using TFTS.Models;

namespace TFTS.Databases
{
    public class PlanDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public PlanDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<RaceModel>().Wait();
        }

        public Task<List<RaceModel>> GetRacePlan() => _database.Table<RaceModel>().ToListAsync();
        public Task<int> SaveRaceToRacePlan(RaceModel data) => _database.InsertAsync(data);
        public Task<int> RemoveRaceFromPlans(RaceModel race) => _database.DeleteAsync(race);/* TODO: crutch, fix it */
        public Task<int> ClearAllPlans() => _database.DeleteAllAsync(new TableMapping(typeof(RaceModel)));
    }
}
