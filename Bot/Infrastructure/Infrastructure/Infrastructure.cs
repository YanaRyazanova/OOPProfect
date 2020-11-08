using System;
using System.Data.SQLite;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;

namespace Infrastructure
{
    public class DataBase
    {
        private static string GetTimetableForGroupForCurrentDay(string groupName, DateTime day)
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var dbName = string.Format(@"{0}\TimeTable.db", path);
            var days = new Dictionary<string, string>
            {
                ["Monday"] = "Понедельник",
                ["Tuesday"] = "Вторник",
                ["Wednesday"] = "Среда",
                ["Thursday"] = "Четверг",
                ["Friday"] = "Пятница",
                ["Saturday"] = "Суббота"
            };
            var connection = new SQLiteConnection(string.Format("Data Source={0};", dbName));
            connection.Open();
            var timetable = new Dictionary<string, string>();
            var command = new SQLiteCommand(string.Format("SELECT timetable FROM TimeTable WHERE group_='{0}' AND dayOfWeek='{1}'",
                                                          groupName, days[day.DayOfWeek.ToString()]), connection);
            var reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                var result = record["timetable"].ToString();
                connection.Close();
                return result;
            }
            return "";
        }

        public static Dictionary<DateTime, string> ParseTimeTable(string groupName)
        {
            var now = DateTime.Now;
            var timetable = GetTimetableForGroupForCurrentDay(groupName, now);
            var result = new Dictionary<DateTime, string>();
            if (timetable.Length > 1)
            {
                var rows = timetable.Split('\n');
                foreach (var row in rows)
                {
                    var timeAndOtherTimesAndProgram = row.Split(':');
                    result[new DateTime(now.Year, now.Month, now.Day,
                           int.Parse(timeAndOtherTimesAndProgram[0]),
                           int.Parse(timeAndOtherTimesAndProgram[1].Split()[0]), 0)]
                           = timeAndOtherTimesAndProgram[timeAndOtherTimesAndProgram.Length - 1].Trim();
                }
            }
            return result;
        }
    }
}