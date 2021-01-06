using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Application;

namespace View
{
    public enum ResponseType
    {
        Help,
        DiningRoom,
        Error,
        StartError,
        Start,
        GroupError,
        SuccessfulRegistration,
        CatchError,
        NotRegisterError,
        RegisterInProgressError,
        RegisterError,
        SucessfulLinks,
        LinksMessage,
        LinksError
    }
    public class DictOfEnumsAndResponses
    {
        public static readonly Dictionary<ResponseType, string> dict = new Dictionary<ResponseType, string>();
        static DictOfEnumsAndResponses()
        {
            dict.Add(ResponseType.Error, $"К сожалению, бот не умеет обрабатывать такую команду :-( Отправьте сообщение 'help' или 'помощь', чтобы увидеть все команды бота./n P.S. Если вы пытались добавить новую ссылку, то она должна быть корректной :-)");
            dict.Add(ResponseType.DiningRoom, "Сейчас в столовой находится вот столько посетителей: ");
            dict.Add(ResponseType.Help, File.ReadAllText("help.txt"));
            dict.Add(ResponseType.StartError, "Начните с команды: \"/start\" ");
            dict.Add(ResponseType.Start, File.ReadAllText("welcome.txt"));
            dict.Add(ResponseType.GroupError, "Такая группа не зарегистрирована в нашем боте. Можешь написать разбаботчикам @barakovskiydef, @yana_rya, @artamaney, @love_3axap и они все сделают :-)");
            dict.Add(ResponseType.SuccessfulRegistration, "Вы успешно зарегистрированы! Выберите пункт меню");
            dict.Add(ResponseType.CatchError, "Упс! Кажется что-то пошло не так!Попробуйте начать с команды '/start'");
            dict.Add(ResponseType.NotRegisterError, "Вы еще не зарегистрированны и вам пока не доступна эта команда. Для начала напишите '/start', а потом выберите свою группу.");
            dict.Add(ResponseType.RegisterInProgressError, "Вы еще не выбрали свою группу");
            dict.Add(ResponseType.RegisterError, "Вы уже зарегистрированы, не стоит использовать эту команду)");
            dict.Add(ResponseType.SucessfulLinks, "Отправьте сообщение в формате:\n*Название чата*: *ссылка*");
            dict.Add(ResponseType.LinksMessage, "Ссылка успешно добавлена");
            dict.Add(ResponseType.LinksError, "Формат ссылки неверный. Ссылка должна содержать 'http(https)://...'");
        }
    }
    public class MessageResponse
    {
        public string response;
        public MessageResponse(ResponseType type)
        {
            response = DictOfEnumsAndResponses.dict[type];
        }
    }
}
