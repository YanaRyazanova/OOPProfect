using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public enum MessagesType
    {
        Start,
        ScheduleForToday,
        ScheduleForTomorrow,
        Help,
        DiningRoom,
        Error
    }
    public class MessageRequest
    {
        public MessagesType type;
        public long userId;
        private Dictionary<string, MessagesType> dict = new Dictionary<string, MessagesType>();

        public MessageRequest(string curTtype, long userId)
        {
            dict["pасписание на сегодня"] = MessagesType.ScheduleForToday;
            dict["pасписание на завтра"] = MessagesType.ScheduleForTomorrow;
            dict["help"] = MessagesType.Help;
            dict["я в столовой"] = MessagesType.DiningRoom;
            foreach (var key in dict.Keys)
            {
                
            }
            if (curTtype == "pасписание на завтра")
                Console.WriteLine("ok");
            try
            { 
                type = dict[curTtype];
            }
            catch (KeyNotFoundException e)
            {
                if (curTtype.Contains("сегодня"))
                    type = MessagesType.ScheduleForToday;
                else if (curTtype.Contains("завтра"))
                    type = MessagesType.ScheduleForTomorrow;
                else
                {
                    Console.WriteLine(e);
                    type = MessagesType.Error;
                }
                
            }
            this.userId = userId;
            Console.WriteLine(curTtype);
        }
    }
}
