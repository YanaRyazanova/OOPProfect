using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class GroupProvider
    {
        public List<string> GetAllGroups() => groups;
        private readonly List<string> groups = new List<string> { "ФТ-201", "ФТ-202" };
    }
}
