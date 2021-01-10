using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure;

namespace Application
{
    public class ScheduleReply : Reply
    {
        public ScheduleReply(Lesson[] lessons)
        {
            this.lessons = lessons;
        }

        public Lesson[] lessons { get; }
    }
}
