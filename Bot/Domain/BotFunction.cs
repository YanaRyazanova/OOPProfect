using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Domain
{
    public abstract class BotFunction
    {
        public readonly string GroupName;
        protected BotFunction(string groupName)
        {
            GroupName = groupName;
        }
    }
}