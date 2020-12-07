using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;


namespace Domain.Functions
{
    public class LessonReminder<TDataBase> : BotFunction
    where TDataBase : ITuple
    {
        private readonly TDataBase nearestLesson;
        public LessonReminder(TDataBase lesson)
        {
            this.nearestLesson = lesson;
        }
        private int GetDifference()
        {
            var lessonStartTime = (DateTime)nearestLesson[0];
            var difference = lessonStartTime.Minute + lessonStartTime.Hour * 60
                             - DateTime.Now.Minute - DateTime.Now.Hour * 60;
            return difference;
        }

        public string Do()
        {
            while (true)
            {
                var difference = GetDifference();
                if (difference <= 10) return $"Через {difference} начнется пара {nearestLesson[1]}";
                var sleepTime = difference - 10;
                Thread.Sleep(sleepTime);
            }
        }
    }
}