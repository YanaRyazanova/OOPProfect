using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class Link
    {
        public readonly string name;
        public readonly string link;

        public Link(string name, string link)
        {
            this.name = name;
            this.link = link;
        }
    }
}