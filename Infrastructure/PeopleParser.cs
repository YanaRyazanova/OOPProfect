using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

namespace Infrastructure
{
    class PeopleParser
    {
        public string GetGroupFromId(string id)
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            path = path.Remove(path.Length - 28);
            var dbName = string.Format(@"{0}\PeopleAndGroups.db", path);
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("SELECT Group FROM PeopleAndGroups WHERE ChatID='{0}'", id), connection);
            var reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                var result = record["ChatID"].ToString();
                connection.Close();
                return result;
            }
            return "";
        }

        public void AddNewUser(string id, string group)
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            path = path.Remove(path.Length - 28);
            var dbName = string.Format(@"{0}\PeopleAndGroups.db", path);
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("INSERT INTO PeopleAndGroups ('ChatID', 'Group') VALUES ('{0}', '{1}')", id, group), connection);
            command.ExecuteNonQuery();
        }
    }
}
