using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure;

namespace Application
{
    public class LinksReply : Reply
    {
        public Link[] links { get; }
        public LinksReply(Link[] links)
        {

        }
    }
}
