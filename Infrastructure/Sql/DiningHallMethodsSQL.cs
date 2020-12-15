using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace Infrastructure.Sql
{
    class DiningHallMethodsSQL
    {
        private readonly DBNameProvider dbNameProvider;
        public DiningHallMethodsSQL(DBNameProvider dBNameProvider)
        {
            this.dbNameProvider = dBNameProvider;
        }

        public void AddPersonToDiningHall(string id)
        {
            var dbName = dbNameProvider.GetDBName("DiningHall");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("INSERT INTO diningHall ('ChatID') VALUES ('{0}')", id), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void DeletePersonFromDiningHall(string id)
        {
            var dbName = dbNameProvider.GetDBName("DiningHall");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("DELETE FROM diningHall where ChatID = '{0}'", id), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public int CountPeopleInDiningHall()
        {
            var dbName = dbNameProvider.GetDBName("DiningHall");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand("SELECT * FROM diningHall", connection);
            var reader = command.ExecuteReader();
            var count = reader.FieldCount;
            connection.Close();
            return count;
        }
    }
}
