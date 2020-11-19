using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Domain.Functions
{
    public class ScheduleSender<TSchedule> : BotFunction
    {
        private readonly TSchedule schedule; 
        public ScheduleSender(TSchedule schedule)
        {
            this.schedule = schedule;
        }

        public TSchedule Do()
        {
            return schedule;
        }
    }
}
