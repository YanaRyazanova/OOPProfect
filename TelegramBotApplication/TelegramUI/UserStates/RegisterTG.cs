using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public class RegisterTG : CommandTG
    {
        private readonly MessageHandler messageHandler;
        private readonly ITGMessageSender tgMessageSender;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private static ReplyKeyboardMarkup CreateKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("Расписание на сегодня"),
                    new KeyboardButton("Расписание на завтра")
                },

                new[]
                {
                    new KeyboardButton("Я в столовой"),
                    new KeyboardButton("Ссылки на учебные чаты")
                },

                new []
                {
                    new KeyboardButton("Добавить ссылку на чат"), 
                    new KeyboardButton("Help")
                }
            });
            return keyboard;
        }

        public RegisterTG(MessageHandler messageHandler,
            ITGMessageSender tgMessageSender,
            TGUnknownMessageProcessor tgUnknownMessageProcessor, RegisterCommandListProvider registerCommandListProvider)
        {
            this.messageHandler = messageHandler;
            this.tgMessageSender = tgMessageSender;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.registerCommandListProvider = registerCommandListProvider;
        }

        public ReplyKeyboardMarkup GetKeyboard()
        {
            return CreateKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            if (messageText.Contains("http"))
            {
                var splittedMessage = messageText.Split(": ");
                var name = splittedMessage[0];
                var link = splittedMessage[1];
                messageHandler.AddLink(user, name, link);
                return;
            }
            
            switch (messageText)
            {
                case "расписание на сегодня":
                {
                    messageHandler.GetScheduleForToday(user);
                    break;
                }
                case "расписание на завтра":
                {
                    messageHandler.GetScheduleForNextDay(user);
                    break;
                }
                case var n when(n == registerCommandListProvider.GetingDiningRoom):
                {
                    var visitorsCount = messageHandler.GetDiningRoom(user);
                    var text = new MessageResponse(ResponseType.DiningRoom).response;
                    tgMessageSender.SendNotification(user, text + visitorsCount, GetKeyboard());
                    break;
                }
                case "ссылки на учебные чаты":
                {
                    messageHandler.GetLinks(user);
                    break;
                }
                case "добавить ссылку на чат":
                {
                    messageHandler.AskForLink(user);
                    break;
                }
                default:
                {
                    tgUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.RegisterError));
                    break;
                }
            }
        }
    }
}