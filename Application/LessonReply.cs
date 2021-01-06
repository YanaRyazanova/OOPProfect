using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class LessonReply : Reply
    {
        public Tuple<string, int> LessonInfo { get; }
        
        public LessonReply(Tuple<string, int> lessonInfo)
        {
            LessonInfo = lessonInfo;
        }
    }
}
