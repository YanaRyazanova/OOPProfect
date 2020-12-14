using System;
using System.Collections.Generic;

namespace Infrastructure.SQL
{
    public class DataBaseSQL
    {
        public static Lesson GetNearestLesson(string groupName)
        {
            var parser = new DataBaseParserSQL(new DBNameProvider());
            var timeTable = parser.GetTimetableForGroupForCurrentDay(groupName, DateTime.Now);
            return new ParseMethods().GetNearestLesson(timeTable);
        }
    }
}