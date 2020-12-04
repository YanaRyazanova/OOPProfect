using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Domain.Functions
{
    public class ScheduleSender : BotFunction
    {
        private readonly string schedule; 
        public ScheduleSender(string schedule)
        {
            this.schedule = schedule;
        }

        public string Do()
        {
            return schedule;
        }
    }
}
