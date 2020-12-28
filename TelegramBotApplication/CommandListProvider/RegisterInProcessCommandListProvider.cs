using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application;

namespace View
{
    public class RegisterInProcessCommandListProvider : CommandListProvider
    {
        private readonly MessageHandler messageHandler;

        public RegisterInProcessCommandListProvider(MessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        public override List<string> GetCommands()
        {
            var groups = messageHandler
                .GetAllGroups()
                .ToList();
            return new List<string> {"help", "/help", "помощь", "помоги"}
                .Concat(groups)
                .ToList();
        }
    }
}
