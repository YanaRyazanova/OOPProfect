using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class NextLesson
    {
        public string LessonName { get; }
        public int TimeToStart { get; }

        public NextLesson(string lessonName, int time)
        {
            LessonName = lessonName;
            TimeToStart = time;
        }
    }
}
