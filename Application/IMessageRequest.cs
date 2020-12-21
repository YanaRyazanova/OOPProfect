using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public enum RequestType
    {
        Start,
        ScheduleForToday,
        ScheduleForTomorrow,
        Help,
        DiningRoom,
        Error,
        Group
    }
    public abstract class IMessageRequest
    {
        public long userId;
        public RequestType type;
        public IMessageRequest() {}
    }
}
