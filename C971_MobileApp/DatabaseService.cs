using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971_MobileApp
{
    internal static class DatabaseService
    {
        private static SQLiteAsyncConnection _database;

        public static async Task InitializeAsync()
        {
            if (_database != null) return;

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "C971.db");
            _database = new SQLiteAsyncConnection(databasePath);

            await _database.CreateTableAsync<Term>();
            await _database.CreateTableAsync<Course>();
        }

        public static SQLiteAsyncConnection Database => _database;
    }
}
