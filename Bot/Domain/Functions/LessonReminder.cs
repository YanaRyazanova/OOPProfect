using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Domain.Functions
{
    public class LessonReminder : BotFunction
    {
        //получать все зависимости в конструкторе
        public string Do()
        {
            var lessonStartTime = GetNextLesson().Item1;
            var lessonName = GetNextLesson().Item2;
            //var lessonStartTime = new DateTime(2020, 11, 9, 22, 31, 0);
            var difference = lessonStartTime.Minute + lessonStartTime.Hour * 60
                             - DateTime.Now.Minute - DateTime.Now.Hour * 60;

            return difference <= 10 ? "Через " + difference + " минут начнется пара:" + lessonName : null;
        }

        private Tuple<DateTime, string> GetNextLesson()
        {
            return DataBase.GetNearestLesson(this.GroupName);
        }
        public LessonReminder(string groupName) : base(groupName) { }
    }
}
//public string Do(DateTime lessonStartTime)
//        {
//            var difference = lessonStartTime.Minute + lessonStartTime.Hour * 60 - DateTime.Now.Minute -
//                             DateTime.Now.Hour * 60;
//            return difference == 10 ? "Скоро начнется пара:" : null;
//        }