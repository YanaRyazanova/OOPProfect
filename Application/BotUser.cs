using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class BotUser
    {
        public string UserId { get; }

        public BotUser(string id)
        {
            UserId = id;
        }
    }
}
