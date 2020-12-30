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

        public Link[] GetActualLinksForGroup(string group) => new ParseMethods().ParseLinks(GetActualLinksForGroupInString(group));

        private string GetActualLinksForGroupInString(string group)
        {
            var dbName = dbNameProvider.GetDBName("link");
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                var command = new SQLiteCommand(string.Format("SELECT links FROM Links WHERE GROUP_='{0}'", group), connection);
                var reader = command.ExecuteReader();
                var emptyLinks = new Link[0];
                foreach (DbDataRecord record in reader)
                {
                    var notParsedLinks = record["links"].ToString();
                    return notParsedLinks;
                }
                return "";
            }
        }

        public void AddLinkForGroup(string group, string name, string link)
        {
            var currentLink = GetActualLinksForGroupInString(group);
            currentLink = $"{currentLink}\n{name}$$${link}";
            var dbName = dbNameProvider.GetDBName("link");
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                var command = new SQLiteCommand(
                        string.Format("UPDATE PeopleAndGroups SET links='{0}' WHERE GROUP_='{1}'", currentLink, group), connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
