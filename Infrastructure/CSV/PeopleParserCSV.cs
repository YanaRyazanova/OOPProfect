using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Infrastructure.Csv
{
    public class PeopleParserCsv
    {
        private readonly DBNameProvider dbNameProvider;
        public PeopleParserCsv(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }

        public string GetGroupFromId(string id)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            using (TextFieldParser parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    for (var i = 0; i < fields.Length - 1; i += 2)
                    {
                        if (fields[i] == id)
                            return fields[i + 1];
                    }
                }
            }
            return "";
        }

        public void AddNewUser(string id, string group)
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            Console.WriteLine(dbName);
            using (var writer = new StreamWriter(dbName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField(id);
                csv.WriteField(group);
            }
        }

        public string[] GetAllUsers()
        {
            var dbName = dbNameProvider.GetDBName("PeopleAndGroups", "csv");
            Console.WriteLine(dbName);
            var users = new List<string>();
            using (TextFieldParser parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(";");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    for (var i = 2; i < fields.Length - 1; i += 2)
                    {
                        users.Add(fields[i]);
                    }
                }
            }
            return users.ToArray();
        }
    }
}
