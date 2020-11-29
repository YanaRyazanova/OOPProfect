using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Domain.Functions
{
    public class ScheduleSender<TLesson> : BotFunction
    {
        private readonly TLesson[] schedule; 
        public ScheduleSender(TLesson[] schedule)
        {
            this.schedule = schedule;
        }

        public TLesson[] Do()
        {
            return schedule;
        }
    }
}
