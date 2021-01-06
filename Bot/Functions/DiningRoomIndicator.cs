using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Functions
{
    public class DiningRoomIndicator
    {
        public int VisitorsCount { get; private set; }
        private Dictionary<string, DateTime> users = new Dictionary<string, DateTime>();
        public DiningRoomIndicator()
        {
            VisitorsCount = 0;
        }
        public void Increment(string id)
        {
            if (!users.ContainsKey(id))
            {
                users.Add(id, DateTime.Now);
                VisitorsCount++;
                Console.WriteLine("Increment for " + id);
                Console.WriteLine(users.Keys + users.Values.ToString());
            }
        }

        public void Decrement(string id)
        {
            if (!users.ContainsKey(id))
                return;
            var difference = DateTime.Now.Hour + DateTime.Now.Minute - users[id].Hour - users[id].Minute;
            if (difference >= 20)
            {
                VisitorsCount--;
                users.Remove(id);
                Console.WriteLine("Decrement for ", id);
            }
        }
    }
}
