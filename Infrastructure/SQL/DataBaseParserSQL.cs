using System;
using System.Data.SQLite;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;

namespace Infrastructure.SQL
{
    public class DataBaseParserSql : IDataBase
    {
        private readonly DBNameProvider dbNameProvider;
        public Lesson[] ParseTimeTable(string timetable, DateTime day) => new ParseMethods().ParseTimeTable(timetable, day);

        public DataBaseParserSql(DBNameProvider dbNameProvider)
        {
            this.dbNameProvider = dbNameProvider;
        }

        public Lesson[] GetTimetableForGroupForCurrentDay(string groupName, DateTime day)
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
            var command = new SQLiteCommand(string.Format("SELECT timetable FROM TimeTable WHERE group_='{0}' AND dayOfWeek='{1}'",
                groupName, days[day.DayOfWeek.ToString()]), connection);
            var reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                var timetableString = record["timetable"].ToString();
                connection.Close();
                return ParseTimeTable(timetableString, day);
            }
            return new Lesson[0];
        }
    }
}