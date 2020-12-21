using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public  interface ILinkParser
    {
        public Link[] GetActualLinksForGroup(string group);
    }
}
