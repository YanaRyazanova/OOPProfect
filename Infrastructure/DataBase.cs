using System;
using System.Collections.Generic;

//namespace Infrastructure
//{
//    public class DataBase
//    {
//        public static Lesson GetNearestLesson(string groupName)
//        {
//            var parser = new DataBaseParser(new DBNameProvider());
//            var now = DateTime.Now;
//            var timeTable = parser.GetTimetableForGroupForCurrentDay(groupName, now);
//            var nearestLesson = new Lesson(DateTime.Now, "Сегодня пар больше нет ^-^");
//            var minDif = long.MaxValue;
//            foreach (var el in timeTable)
//            {
//                var time = el.time;
//                var dif = time.Hour * 60 + time.Minute - now.Hour * 60 - now.Minute;
//                if (dif < minDif && dif > 0)
//                {
//                    minDif = dif;
//                    nearestLesson = el;
//                }
//            }
//            return nearestLesson;
//        }
//    }
//}