using System;
using System.Collections.Generic;
using System.Text;


namespace Domain.Functions
{
    public class LessonReminder<TLesson> : BotFunction
    {
        private readonly DateTime nearestLessonTime;
        private readonly string nearestLessonName;
        public LessonReminder(DateTime time, string name)
        {
            nearestLessonTime = time;
            nearestLessonName = name;
        }
        private string GetInformationString()
        {
            var lessonStartTime = nearestLessonTime;
            var lessonName = nearestLessonName;
            var difference = lessonStartTime.Minute + lessonStartTime.Hour * 60
                             - DateTime.Now.Minute - DateTime.Now.Hour * 60;

            return difference <= 10 ? "Через " + difference + " минут начнется пара:" + lessonName : null;
        }
        public string Do(TLesson nearestLesson)
        {
            while (true)
            {
                var res = GetInformationString();
                if (res != null)
                    return res;
            }
        }
    }
}