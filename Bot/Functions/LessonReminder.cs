using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;


namespace Domain.Functions
{
    public class LessonReminder: BotFunction
    {
        //private readonly TDataBase nearestLesson;
        public LessonReminder(
            //TDataBase lesson
            )
        {
            //this.nearestLesson = lesson;
        }
        private int GetDifference(DateTime startTime)
        {
            var difference = startTime.Minute + startTime.Hour * 60
                             - DateTime.Now.Minute - DateTime.Now.Hour * 60;
            return difference;
        }

        public string Do(DateTime startTime, string name)
        {
            while (true)
            {
                var difference = GetDifference(startTime);
                if ( difference <= 10 && difference >=0) return $"Через {difference} начнется пара {name}";
                var sleepTime = difference - 10;
                if (difference < 0)
                    return null;
                //Thread.Sleep(sleepTime);
            }
        }
    }
}