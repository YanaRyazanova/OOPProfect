﻿using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;
using Ninject;

namespace View
{
    public class NotRegisterTG : CommandTG
    {
        private readonly StandardKernel container;
        private readonly TGMessageSender tgMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;

        private static ReplyKeyboardMarkup CreatePreStartKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("Начать")
                }
            });
            return keyboard;
        }

        public NotRegisterTG(TGMessageSender tgMessageSender,
            IPeopleParser peopleParser,
            TGUnknownMessageProcessor tgUnknownMessageProcessor,
            StandardKernel container)
        {
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.container = container;
        }

        public ReplyKeyboardMarkup GetKeyboard()
        {
            return CreatePreStartKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            switch (messageText)
            {
                case "/start":
                case "start":
                case "начать":
                {
                    var text = new MessageResponse(ResponseType.Start).response;
                    peopleParser.AddNewUser(user.UserId, "tg");
                    peopleParser.EvaluateState(user.UserId);
                    var updatedState = container.Get<RegisterInProcessTG>();
                    tgMessageSender.SendNotification(user, text, updatedState.GetKeyboard());
                    break;
                }
                default:
                    tgUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.NotRegisterError));
                    break;
            }
        }
    }
}
