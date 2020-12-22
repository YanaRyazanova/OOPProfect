﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
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
            var notParsedLinks = "";
            using (TextFieldParser parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields[0] == group)
                        notParsedLinks = fields[1];
                    return new ParseMethods().ParseLinks(notParsedLinks.Replace(@"\n", "\n"));
                }
            }
            return new Link[0];
        }
    }
}