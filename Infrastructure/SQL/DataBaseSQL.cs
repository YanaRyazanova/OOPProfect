using System;
using System.Collections.Generic;

namespace Infrastructure.SQL
{
    public class DataBaseSql
    {
        public static Lesson GetNearestLesson(string groupName)
        {
            var parser = new DataBaseParserSql(new DBNameProvider());
            var timeTable = parser.GetTimetableForGroupForCurrentDay(groupName, DateTime.Now);
            return new ParseMethods().GetNearestLesson(timeTable);
        }
    }
}