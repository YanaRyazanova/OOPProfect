using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public interface CommandListProvider
    {
        public List<string> GetCommands();
    }
}
