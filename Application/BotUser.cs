using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class BotUser
    {
        public string Domain { get; }
        public string UserId { get; }

        public BotUser(string id, string domain)
        {
            UserId = id;
            Domain = domain;
        }
    }
}
