using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


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
        private string GetInformationString()
        {
            var lessonStartTime = (DateTime)nearestLesson[0];
            var difference = lessonStartTime.Minute + lessonStartTime.Hour * 60
                             - DateTime.Now.Minute - DateTime.Now.Hour * 60;
            return difference <= 10 ? $"Через {difference} начнется пара {nearestLesson[1]}": null;
        }
        public string Do()
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