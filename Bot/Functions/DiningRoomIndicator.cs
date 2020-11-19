using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Functions
{
    public class DiningRoomIndicator : BotFunction
    {
        public int VisitorsCount { get; private set; }
        public DiningRoomIndicator()
        {
            VisitorsCount = 0;
        }
        public void Increment()
        {
            VisitorsCount++;
        }
    }
}
