using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public enum ResponseType
    {
        Help,
        DiningRoom,
        Error,
        StartError,
        Start
    }

    public abstract class IMessageResponse
    {
        public string response;
        public IMessageResponse() {}
    }
}
