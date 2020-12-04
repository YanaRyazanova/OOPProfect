﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

namespace Infrastructure
{
    class PeopleParser
    {
        private readonly DBNameProvider dbNameProvider;
        public PeopleParser(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }
        public string GetGroupFromId(string id)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("SELECT GROUP_ FROM PeopleAndGroups WHERE ChatID='{0}'", id), connection);
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
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("INSERT INTO PeopleAndGroups ('ChatID', 'GROUP_') VALUES ('{0}', '{1}')", id, group), connection);
            command.ExecuteNonQuery();
        }
    }
}
