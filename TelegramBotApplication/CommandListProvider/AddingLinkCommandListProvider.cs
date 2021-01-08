using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public class AddingLinkCommandListProvider : CommandListProvider
    {
        public List<string> GetCommands()
        {
            return new List<string>{"назад"};
        }
    }
}
