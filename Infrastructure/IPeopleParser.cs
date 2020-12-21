using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public interface IPeopleParser
    {
        public string GetGroupFromId(string id);
        public string GetStateFromId(string id);
        public void AddNewUser(string id, string group, string state);
        public string[] GetAllUsers();
    }
}
