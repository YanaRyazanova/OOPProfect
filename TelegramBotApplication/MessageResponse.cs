﻿using System;
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
        SucceessfulRegistration,
        CatchError,
        NotRegisterError,
        RegisterInProgressError,
        RegisterError
    }
    public class DictOfEnumsAndResponses
    {
        public static readonly Dictionary<ResponseType, string> dict = new Dictionary<ResponseType, string>();
        static DictOfEnumsAndResponses()
        {
            dict.Add(ResponseType.Error, "К сожалению, бот не умеет обрабатывать такую команду :-( Отправьте сообщение 'help' или 'помощь', чтобы увидеть все команды бота.");
            dict.Add(ResponseType.DiningRoom, "Сейчас в столовой находится вот столько посетителей: ");
            dict.Add(ResponseType.Help, File.ReadAllText("help.txt"));
            dict.Add(ResponseType.StartError, "Начните с команды: \"/start\" ");
            dict.Add(ResponseType.Start, File.ReadAllText("welcome.txt"));
            dict.Add(ResponseType.GroupError, "Такая группа не зарегистрирована в нашем боте. Можешь написать разбаботчикам @barakovskiydef, @yana_rya, @artamaney, @love_3axap и они все сделают :-)");
            dict.Add(ResponseType.SucceessfulRegistration, "Вы успешно зарегистрированы! Выберите пункт меню");
            dict.Add(ResponseType.CatchError, "Упс! Кажется что-то пошло не так!Попробуйте начать с команды '/start'");
            dict.Add(ResponseType.NotRegisterError, "Вы еще не зарегистрированны и вам пока не доступна эта команда. Для начала напишите '/start', а потом выберите свою группу.");
            dict.Add(ResponseType.RegisterInProgressError, "Вы еще не выбрали свою группу");
            dict.Add(ResponseType.RegisterError, "Вы уже зарегистрированы, не стоит использовать эту команду)");
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
