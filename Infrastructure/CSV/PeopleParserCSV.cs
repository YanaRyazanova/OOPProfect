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
                        return fields[2];
                    }
                }
            }
            return "ФТ-202";
        }

        public void AddNewUser(string id, string group, string state)
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
                csv.WriteField(group);
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