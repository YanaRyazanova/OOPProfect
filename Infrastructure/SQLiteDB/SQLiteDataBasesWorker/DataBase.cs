using System;
using System.Collections.Generic;

namespace Infrastructure
{
    public class DataBase
    {
        public Lesson GetNearestLesson(string groupName)
        {
            var parser = new DataBaseParser(new DBNameProvider());
            var timeTable = parser.GetTimetableForGroupForCurrentDay(groupName, DateTime.Now);
            return new ParseMethods().GetNearestLesson(timeTable);
        }
    }
}