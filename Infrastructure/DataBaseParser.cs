using System;
using System.Data.SQLite;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;

namespace Infrastructure
{
    public class DataBaseParser
    {
        private readonly DBNameProvider dbNameProvider;

        public DataBaseParser(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }

        public string GetTimetableForGroupForCurrentDay(string groupName, DateTime day)
        {
            var dbName = dbNameProvider.GetDBName("TimeTable");
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
    }
}