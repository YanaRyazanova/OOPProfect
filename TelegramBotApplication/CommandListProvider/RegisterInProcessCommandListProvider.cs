using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application;

namespace View
{
    public class RegisterInProcessCommandListProvider : CommandListProvider
    {
        private readonly GroupProvider groupProvider;

        public RegisterInProcessCommandListProvider(GroupProvider groupProvider)
        {
            this.groupProvider = groupProvider;
        }

        public override List<string> GetCommands()
        {
            var groups = groupProvider
                .GetAllGroups()
                .ToList();
            return new List<string> {"help", "/help", "помощь", "помоги"}
                .Concat(groups)
                .ToList();
        }
    }
}
