using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class LinksReply : Reply
    {
        public string Link { get; }

        public LinksReply(string link)
        {
            Link = link;
        }
    }
}
