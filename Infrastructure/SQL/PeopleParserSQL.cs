﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

namespace Infrastructure.SQL
{
    public class PeopleParserSql : IPeopleParser
    {
        private readonly DBNameProvider dbNameProvider;
        public PeopleParserSql(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }

        public string GetPlatformFromId(string id) => GetThingFromId(id, "platform");
        public string GetGroupFromId(string id) => GetThingFromId(id, "GROUP_");
        public string GetStateFromId(string id) => GetThingFromId(id, "State");
        public void EvaluateState(string id)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            var currentState = GetStateFromId(id);
            var statesChanges = new Dictionary<string, string>
            {
                [""] = "0",
                ["0"] = "1",
                ["1"] = "2",
                ["2"] = "3",
                ["3"] = "3"
            };
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (var command =
                    new SQLiteCommand(
                        string.Format("UPDATE PeopleAndGroups SET State='{0}' WHERE ChatID='{1}'",
                            statesChanges[currentState], id), connection))
                {
                    try
                    {
                        ;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        private void ChangeThing(string id, string thing, string whatToChange)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                var command =
                    new SQLiteCommand(
                        string.Format("UPDATE PeopleAndGroups SET {0}='{1}' WHERE ChatID='{2}'", whatToChange, thing, id),
                        connection);
                command.ExecuteNonQuery();
            }
        }
        private int DefineStateFromEnum(UserStates enumState)
        {
            var statesDict = new Dictionary<UserStates, int>
            {
                [UserStates.NotRegister] = 0,
                [UserStates.RegisterInProcess] = 1,
                [UserStates.Register] = 2,
                [UserStates.AddingLink] = 3
            };
            return statesDict[enumState];
        }
        public void ChangeGroup(string id, string group) => ChangeThing(id, group, "GROUP_");
        public void ChangeState(string id, UserStates newState) => ChangeThing(id, DefineStateFromEnum(newState).ToString(), "State");

        private string GetThingFromId(string id, string thingToGet)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (var command =
                    new SQLiteCommand(
                        string.Format("SELECT {0} FROM PeopleAndGroups WHERE ChatID='{1}'", thingToGet, id),
                        connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        foreach (DbDataRecord record in reader)
                        {
                            var result = record[thingToGet].ToString();
                            return result;
                        }
                        return "";
                    }
                }
            }
        }

        public void AddNewUser(string id, string platform, UserStates stateEnum = UserStates.NotRegister)
        {
            var statesDict = new Dictionary<UserStates, int>
            {
                [UserStates.NotRegister] = 0,
                [UserStates.RegisterInProcess] = 1,
                [UserStates.Register] = 2,
                [UserStates.AddingLink] = 3
            };
            var state = statesDict[stateEnum];
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (var command =
                    new SQLiteCommand(string.Format(
                        "INSERT INTO PeopleAndGroups ('ChatID', 'GROUP_', 'State', 'platform') VALUES ('{0}', '{1}', '{2}', '{3}')",
                        id, " ", state, platform), connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Hello from AddNewUser");
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "constraint failed\r\nUNIQUE constraint failed: PeopleAndGroups.ChatID")
                            return;
                        Console.WriteLine(e);
                    }
                }
            }
        }

        public string[] GetAllUsers()
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups");
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT ChatID FROM PeopleAndGroups", connection);
                var reader = command.ExecuteReader();
                var users = new List<string>();
                foreach (DbDataRecord record in reader)
                {
                    users.Add((string)record["ChatID"]);
                }
                return users.ToArray();
            }
        }
    }
}