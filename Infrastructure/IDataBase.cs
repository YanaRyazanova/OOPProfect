using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    interface IDataBase
    {
        Lesson[] ParseTimeTable(string groupName, DateTime day);
    }
}