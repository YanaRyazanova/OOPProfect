﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public interface IDataBaseParser
    {
        public Lesson[] GetTimetableForGroupForCurrentDay(string groupName, DateTime day);
    }
}
