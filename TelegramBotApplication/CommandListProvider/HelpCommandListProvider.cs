using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public class HelpCommandListProvider : CommandListProvider
    {
        public  List<string> GetCommands()
        {
            return new List<string> { "help", "/help", "помощь", "помоги" };
        }
    }
}
