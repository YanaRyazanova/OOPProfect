using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

namespace Infrastructure.SQL
{
    public class PeopleParserSql
    {
        private readonly DBNameProvider dbNameProvider;
        public PeopleParserSql(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }
        public string GetGroupFromId(string id) => GetThingFromId(id, "GROUP_");
        public string GetStateFromId(string id) => GetThingFromId(id, "State");
        private string GetThingFromId(string id, string thingToGet)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("SELECT {0} FROM PeopleAndGroups WHERE ChatID='{1}'", thingToGet, id), connection);
            var reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                var result = record[thingToGet].ToString();
                connection.Close();
                return result;
            }
            return "";
        }

        public void AddNewUser(string id, string group, string state="not_register")
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("INSERT INTO PeopleAndGroups ('ChatID', 'GROUP_', 'State') VALUES ('{0}', '{1}', '{2}')",
                                            id, group, state), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public string[] GetAllUsers()
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand("SELECT ChatID FROM PeopleAndGroups", connection);
            var reader = command.ExecuteReader();
            var users = new List<string>();
            foreach (DbDataRecord record in reader)
            {
                users.Add((string)record["ChatID"]);
            }
            connection.Close();
            return users.ToArray();
        }


    }
}