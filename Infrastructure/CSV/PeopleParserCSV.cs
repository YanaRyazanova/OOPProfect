using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Infrastructure.Csv
{
    public class PeopleParserCsv : IPeopleParser
    {
        private readonly DBNameProvider dbNameProvider;
        public PeopleParserCsv(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }

        public void ChangeStateForUser(string id)
        {
            var group = GetGroupFromId(id);
            var prevState = GetStateFromId(id);
            var states = new Dictionary<string, string>
            {
                [""] = "0",
                ["0"] = "1",
                ["1"] = "2",
                ["2"] = "2",
            };
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            RewriteFields(id, group, states[prevState], dbName);
        }

        public void ChangeGroup(string id, string group)
        {
            var prevState = GetStateFromId(id);
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            RewriteFields(id, group, prevState, dbName);
        }

        private void RewriteFields(string id, string group, string state, string dbName)
        {
            var values = File.ReadAllLines(dbName);
            for (var i = 0; i < values.Length; i++)
            {
                var line = values[i].Split(',');
                if (line[0] == id)
                    values[i] = $"{id},{group},{state}";
            }
            using (StreamWriter Writer = new StreamWriter(dbName, false))
            {
                for (int i = 0; i < values.Length; i++)
                {
                    Writer.WriteLine(values[i]);
                }
            }
        }

        public string getPlatformFromId(string id) => GetThingFromId(id, "platform");
        public string GetGroupFromId(string id) => GetThingFromId(id, "group");
        public string GetStateFromId(string id) => GetThingFromId(id, "state");
        private string GetThingFromId(string id, string thingToGet)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            using (TextFieldParser parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields[0] == id)
                    {
                        if (thingToGet == "group")
                            return fields[1];
                        else if (thingToGet == "state")
                            return fields[2];
                        else
                            return fields[3];
                    }
                }
            }
            return "";
        }

        public void AddNewUser(string id, string state="0", string platform="tg")
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            using (TextFieldParser parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields[0] == id)
                        return;
                }
            }
            using (var writer = File.AppendText(dbName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField(id);
                csv.WriteField(" ");
                csv.WriteField(state);
                csv.WriteField(platform);
                csv.NextRecord();
            }
        }

        public string[] GetAllUsers()
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            var users = new List<string>();
            using (TextFieldParser parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    users.Add(fields[0]);
                }
            }
            return users.ToArray();
        }
    }
}