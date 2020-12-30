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
            TGUnknownMessageProcessor tgUnknownMessageProcessor) : base(TgUsersStates.Register)
        {
            this.messageHandler = messageHandler;
            this.tgMessageSender = tgMessageSender;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
        }

        public override ReplyKeyboardMarkup GetKeyboard()
        {
            return CreateKeyboard();
        }

        public override void ProcessMessage(string messageText, BotUser user)
        {
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
                case "я в столовой":
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
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                {
                    tgMessageSender.HandleHelpMessage(user, GetKeyboard());
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