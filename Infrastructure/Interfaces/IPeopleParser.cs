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
        public void AddNewUser(string id, string platform, string state = "0");
        public void ChangeGroup(string id, string group);
        public void EvaluateState(string id);
        public void ChangeState(string id, string newState);
        public string[] GetAllUsers();
    }
}
