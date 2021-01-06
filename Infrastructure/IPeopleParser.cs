using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public interface IPeopleParser
    {
        public string GetPlatformFromId(string id);
        public string GetGroupFromId(string id);
        public string GetStateFromId(string id);
        public void AddNewUser(string id, string state = "0", string platform="tg");
        public void ChangeGroup(string id, string group);
        public void ChangeStateForUser(string id);
        public string[] GetAllUsers();
    }
}
