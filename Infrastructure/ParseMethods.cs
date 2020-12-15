using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    class ParseMethods
    {
        public Lesson[] ParseTimeTable(string timetable, DateTime day)
        {
            var result = new List<Lesson>();
            if (timetable.Length > 1)
            {
                timetable = timetable.Replace(@"\n", "\n");
                var rows = timetable.Split('\n');
                foreach (var row in rows)
                {
                    var timeAndOtherTimesAndProgram = row.Split(':');
                    result.Add(new Lesson(new DateTime(day.Year, day.Month, day.Day,
                            int.Parse(timeAndOtherTimesAndProgram[0]),
                            int.Parse(timeAndOtherTimesAndProgram[1].Split()[0]), 0),
                        timeAndOtherTimesAndProgram[timeAndOtherTimesAndProgram.Length - 1].Trim()));
                }
            }
            return result.ToArray();
        }

        public Lesson GetNearestLesson(Lesson[] timeTable)
        {
            var now = DateTime.Now;
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