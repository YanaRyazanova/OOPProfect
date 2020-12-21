using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Application;

namespace View
{
    public class DictOfEnumsAndResponses
    {
        public static readonly Dictionary<ResponseType, string> dict = new Dictionary<ResponseType, string>();
        static DictOfEnumsAndResponses()
        {
            dict.Add(ResponseType.Error, "К сожалению, бот не обрабатывает такую команду." +
                                         "Нажмите \"help\", чтобы увидеть все команды");
            dict.Add(ResponseType.DiningRoom, "Сейчас в столовой находится вот столько посетителей: ");
            dict.Add(ResponseType.Help, File.ReadAllText("help.txt"));
            dict.Add(ResponseType.StartError, "Начните с команды: \"/start\" ");
            dict.Add(ResponseType.Start, File.ReadAllText("welcome.txt"));
        }
    }
    public class MessageResponse : IMessageResponse
    {
        public string response;
        public MessageResponse(ResponseType type)
        {
            response = DictOfEnumsAndResponses.dict[type];
        }
    }
}
