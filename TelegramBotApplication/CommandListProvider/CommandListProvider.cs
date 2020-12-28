using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public abstract class CommandListProvider
    {
        public abstract List<string> GetCommands();
    }
}
