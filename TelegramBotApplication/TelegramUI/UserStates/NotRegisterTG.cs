using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public class NotRegisterTG : CommandTG
    {
        private readonly ITGMessageSender tgMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        private readonly MessageHandler messageHandler;
        private readonly GroupProvider groupProvider;
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

        public NotRegisterTG(ITGMessageSender tgMessageSender,
            IPeopleParser peopleParser,
            TGUnknownMessageProcessor tgUnknownMessageProcessor, MessageHandler messageHandler, GroupProvider groupProvider)
        {
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.messageHandler = messageHandler;
            this.groupProvider = groupProvider;
        }

        public override ReplyKeyboardMarkup GetKeyboard()
        {
            return CreatePreStartKeyboard();
        }

        public override void ProcessMessage(string messageText, BotUser user)
        {
            switch (messageText)
            {
                case "/start":
                case "start":
                case "начать":
                {
                    var text = new MessageResponse(ResponseType.Start).response;
                    peopleParser.AddNewUser(user.UserId);
                    peopleParser.ChangeStateForUser(user.UserId);
                    var updatedState = new RegisterInProcessTG(messageHandler, tgMessageSender, peopleParser, tgUnknownMessageProcessor, groupProvider);
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
