using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;


namespace Domain.Functions
{
    public class LessonReminder
    {
        public LessonReminder()
        {

        }
        private int GetDifference(DateTime startTime)
        {
            var difference = startTime.Minute + startTime.Hour * 60
                             - DateTime.Now.Minute - DateTime.Now.Hour * 60;
            return difference;
        }

        public NextLesson Do(DateTime startTime, string name)
        {
            while (true)
            {
                var difference = GetDifference(startTime);
                if (difference <= 10 && difference >=0)
                {
                    var nextLesson = new NextLesson(name, difference);
                    return nextLesson;
                    //return $"Пара '{name}' начнется через столько минут: {difference}";
                }
                if (difference < 0)
                    return null;
            }
        }
    }
}