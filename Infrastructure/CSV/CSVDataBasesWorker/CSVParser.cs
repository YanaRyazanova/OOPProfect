using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic.FileIO;

namespace Infrastructure
{
    class CSVParser : IDataBase
    {
        private readonly DBNameProvider dbNameProvider;
        private readonly string extension;
        public Lesson[] ParseTimeTable(string timetable, DateTime day) => new ParseMethods().ParseTimeTable(timetable, day);

        public CSVParser(DBNameProvider dbNameProvider, string extension = "csv")
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
            var now = DateTime.Now;
            var timeTable = GetTimetableForGroupForCurrentDay(groupName, now);
            var nearestLesson = new Lesson(DateTime.Now, "Сегодня пар больше нет ^-^");
            var minDif = long.MaxValue;
            foreach (var el in timeTable)
            {
                var time = el.time;
                var dif = time.Hour * 60 + time.Minute - now.Hour * 60 - now.Minute;
                if (dif < minDif && dif > 0)
                {
                    minDif = dif;
                    nearestLesson = el;
                }
            }
            return nearestLesson;
        }
    }
}
