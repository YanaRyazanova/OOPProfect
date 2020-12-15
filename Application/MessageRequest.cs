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
    public class MessageRequest
    {
        public MessagesType type;
        public long userId;
        private Dictionary<string, MessagesType> dict = new Dictionary<string, MessagesType>();

        public MessageRequest(string type, long userId)
        {
            dict.Add("Расписание на сегодня", MessagesType.ScheduleForToday);
            dict.Add("Расписание на завтра", MessagesType.ScheduleForTomorrow);
            dict.Add("Help", MessagesType.Help);
            dict.Add("Я в столовой", MessagesType.DiningRoom);
            dict.Add("/start", MessagesType.Start);
            this.type = dict[type];
            this.userId = userId;
        }
    }
}
