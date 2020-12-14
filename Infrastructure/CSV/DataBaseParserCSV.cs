using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic.FileIO;

namespace Infrastructure.CSV
{
    class DataBaseParserCSV : IDataBase
    {
        private readonly DBNameProvider dbNameProvider;
        private readonly string extension;
        public Lesson[] ParseTimeTable(string timetable, DateTime day) => new ParseMethods().ParseTimeTable(timetable, day);

        public DataBaseParserCSV(DBNameProvider dbNameProvider, string extension = "csv")
        {
            this.dbNameProvider = dbNameProvider;
            this.extension = extension;
        }

        public Lesson[] GetTimetableForGroupForCurrentDay(string group, DateTime day)
        {
            var dbName = dbNameProvider.GetDBName("TimeTable", extension);
            var days = new Dictionary<string, string>
            {
                ["Monday"] = "Понедельник",
                ["Tuesday"] = "Вторник",
                ["Wednesday"] = "Среда",
                ["Thursday"] = "Четверг",
                ["Friday"] = "Пятница",
                ["Saturday"] = "Суббота"
            };
            var lessons = "";
            using (TextFieldParser parser = new TextFieldParser(dbName))
            {
                parser.SetDelimiters(";");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    for (var i = 3; i < fields.Length - 2; i += 3)
                    {
                        if (fields[i] == days[day.DayOfWeek.ToString()] && fields[i + 1] == group)
                        {
                            return ParseTimeTable(fields[i + 2], day);
                        }
                    }
                }
            }
            return new Lesson[0];
        }

        public Lesson GetNearestLesson(string groupName)
        {
            var timeTable = GetTimetableForGroupForCurrentDay(groupName, DateTime.Now);
            return new ParseMethods().GetNearestLesson(timeTable);
        }
    }
}
