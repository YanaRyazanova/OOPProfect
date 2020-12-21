using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace Infrastructure.SQL
{
    public class LinkParserSQL : ILinkParser
    {
        private readonly DBNameProvider dbNameProvider;

        public LinkParserSQL(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }

        public Link[] GetActualLinksForGroup(string group)
        {
            var dbName = dbNameProvider.GetDBName("link");
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var command = new SQLiteCommand(string.Format("SELECT links FROM TimeTable WHERE GROUP_='{0}'", group), connection);
            var reader = command.ExecuteReader();
            var emptyLinks = new Link[0];
            foreach (DbDataRecord record in reader)
            {
                var notParsedLinks = record["links"].ToString();
                connection.Close();
                return new ParseMethods().ParseLinks(notParsedLinks);
            }
            connection.Close();
            return emptyLinks;
        }
    }
}