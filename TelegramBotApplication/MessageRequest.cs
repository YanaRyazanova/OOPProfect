using System;
using System.Collections.Generic;
using System.Text;
using Application;

namespace View
{
    public class DictOfMessagesAndEnums
    {
        public static readonly Dictionary<string, RequestType> dict = new Dictionary<string, RequestType>();

        static DictOfMessagesAndEnums()
        {
            dict["pасписание на сегодня"] = RequestType.ScheduleForToday;
            dict["pасписание на завтра"] = RequestType.ScheduleForTomorrow;
            dict["help"] = RequestType.Help;
            dict["я в столовой"] = RequestType.DiningRoom;
        }
    }
    public class MessageRequest : IMessageRequest
    {
        public RequestType type;
        public long userId;
        public MessageRequest(string curType, long userId)
        {
            try
            {
                type = DictOfMessagesAndEnums.dict[curType];
            }
            catch (KeyNotFoundException e)
            {
                if (curType.Contains("сегодня"))
                    type = RequestType.ScheduleForToday;
                else if (curType.Contains("завтра"))
                    type = RequestType.ScheduleForTomorrow;
                else
                {
                    Console.WriteLine(e);
                    type = RequestType.Error;
                }
            }
            this.userId = userId;
        }
    }
}