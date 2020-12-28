using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public class NotRegicterCommandListProvider : CommandListProvider
    {
        public override List<string> GetCommands()
        {
            return new List<string> {"/start", "start", "начать", "help", "/help", "помощь", "помоги"};
        }
    }
}
