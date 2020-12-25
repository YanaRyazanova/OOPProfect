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
            string[] values = File.ReadAllLines(dbName);
            using (StreamWriter Writer = new StreamWriter(dbName, false))
            {
                for (int i = 0; i < values.Length; i++)
                {
                    Writer.WriteLine(values[i].Replace($"{id},{group},{prevState}", $"{id},{group},{states[prevState]}"));
                }
            }
        }

        public void ChangeGroup(string id, string group)
        {
            var prevGrpoup = GetGroupFromId(id);
            var prevState = GetStateFromId(id);
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            string[] values = File.ReadAllLines(dbName);
            using (StreamWriter Writer = new StreamWriter(dbName, false))
            {
                for (int i = 0; i < values.Length; i++)
                {
                    Writer.WriteLine(values[i].Replace($"{id},{prevGrpoup},{prevState}", $"{id},{group},{prevState}"));
                }
            }
        }

        public string GetGroupFromId(string id) => GetThingFromId(id, "group");
        public string GetStateFromId(string id) => GetThingFromId(id, "state");
        private string GetThingFromId(string id, string thingToGet)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            using (TextFieldParser parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(";");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields[0] == id)
                    {
                        if (thingToGet == "group")
                            return fields[1];
                        return fields[2];
                    }
                }
            }
            return "";
        }

        public void AddNewUser(string id, string state="0")
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