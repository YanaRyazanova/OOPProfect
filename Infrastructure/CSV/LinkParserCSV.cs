using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.Csv
{
    public class LinkParserCSV : ILinkParser
    {
        private readonly DBNameProvider dbNameProvider;
        private readonly string extension;

        public LinkParserCSV(DBNameProvider dbNameProvider, string extension = "csv")
        {
            this.dbNameProvider = dbNameProvider;
            this.extension = extension;
        }

        public Link[] GetActualLinksForGroup(string group)
        {
            var dbName = dbNameProvider.GetDBName("link", extension);
            using (var parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields[0] != @group) continue;
                    var notParsedLinks = fields[1];
                    return new ParseMethods().ParseLinks(notParsedLinks.Replace(@"\n", "\n"));
                }
            }
            
            return new Link[0];
        }

        public void AddLinkForGroup(string group, string name, string link)
        {
            var dbName = dbNameProvider.GetDBName("link", extension);
            var values = File.ReadAllLines(dbName);
            for (var i = 0; i < values.Length; i++)
            {
                var line = values[i].Split(',');
                if (line[0] == group)
                    values[i] = $"{values[i]}\\n{name}$$${link}";
            }
            using (var Writer = new StreamWriter(dbName, false))
            {
                for (var i = 0; i < values.Length; i++)
                    Writer.WriteLine(i);
            }
        }
    }
}
