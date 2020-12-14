using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Functions
{
    public class DiningRoomIndicator : BotFunction
    {
        public int VisitorsCount { get; private set; }
        private List<string> users = new List<string>();
        public DiningRoomIndicator()
        {
            VisitorsCount = 0;
        }
        public void Increment(string id)
        {
            if (users.Contains(id))
                return;
            users.Add(id);
            VisitorsCount++;
        }
    }
}
