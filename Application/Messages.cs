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
        DiningRoom
    }
    public class Messages
    {
        public MessagesType type;
        public long userId;
        private Dictionary<string, MessagesType> dict = new Dictionary<string, MessagesType>();

        public Messages(string type, long userId)
        {
            dict.Add("расписание на сегодня", MessagesType.ScheduleForToday);
            dict.Add("расписание на завтра", MessagesType.ScheduleForTomorrow);
            dict.Add("help", MessagesType.Help);
            dict.Add("я в столовой", MessagesType.DiningRoom);
            this.type = dict[type];
            this.userId = userId;
        }
    }
}
