using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public enum ResponseType
    {
        Help,
        DiningRoom,
        Error
    }
    public class MessageResponse
    {
        public string response;
        private Dictionary<ResponseType, string> dict = new Dictionary<ResponseType, string>();
        public MessageResponse(ResponseType type)
        {
            dict.Add(ResponseType.Error, "К сожалению, бот не обрабатывает такую команду." +
                     "Нажмите \"help\", чтобы увидеть все команды");
            dict.Add(ResponseType.DiningRoom, "Сейчас в столовой находится вот столько посетителей: ");
            dict.Add(ResponseType.Help, "help");
            response = dict[type];

        }
    }
}
