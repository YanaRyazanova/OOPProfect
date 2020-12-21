using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public interface IPeopleParser
    {
        public string GetGroupFromId(string id);
        public void AddNewUser(string id, string group);
        public string[] GetAllUsers();
    }
}
