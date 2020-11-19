using System;
using System.Collections.Generic;
using System.Text;


namespace Domain.Functions
{
    public class LessonReminder : BotFunction
    {
        private readonly DateTime nearestLessonTime;
        private readonly string nearestLessonName;
        public LessonReminder(DateTime time, string name)
        {
            nearestLessonTime = time;
            nearestLessonName = name;
        }
        public string Do()
        {
            var lessonStartTime = nearestLessonTime;
            var lessonName = nearestLessonName;
            var difference = lessonStartTime.Minute + lessonStartTime.Hour * 60
                             - DateTime.Now.Minute - DateTime.Now.Hour * 60;

            return difference <= 10 ? "Через " + difference + " минут начнется пара:" + lessonName : null;
        }
    }
}