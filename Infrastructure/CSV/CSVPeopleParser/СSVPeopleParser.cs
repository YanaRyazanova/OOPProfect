using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    class CSVPeopleParser
    {
        private readonly DBNameProvider dbNameProvider;
        public CSVPeopleParser(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }

        public string GetGroupFromId(string id)
        {
            return "";
        }

        public void AddNewUser(string id, string group)
        {
            
        }

        public string[] GetAllUsers()
        {
            return new string[0];
        }
    }
}
