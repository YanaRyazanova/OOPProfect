using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public enum UserStates
    {
        NotRegister,
        RegisterInProcess,
        Register,
        AddingLink
    }
    public interface IPeopleParser
    {
        public string GetPlatformFromId(string id);
        public string GetGroupFromId(string id);
        public string GetStateFromId(string id);
        public void AddNewUser(string id, string platform, UserStates state = UserStates.NotRegister);
        public void ChangeGroup(string id, string group);
        public void EvaluateState(string id);
        public void ChangeState(string id, UserStates newState);
        public string[] GetAllUsers();
    }
}
