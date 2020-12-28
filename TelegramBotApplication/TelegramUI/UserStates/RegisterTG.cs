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
        private readonly TGMessageSender tgMessageSender;
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
                    new KeyboardButton("Help")
                }
            });
            return keyboard;
        }

        public RegisterTG(MessageHandler messageHandler, TGMessageSender tgMessageSender, TGUnknownMessageProcessor tgUnknownMessageProcessor) : base(TgUsersStates.Register)
        {
            this.messageHandler = messageHandler;
            this.tgMessageSender = tgMessageSender;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
        }

        public override ReplyKeyboardMarkup GetKeyboard()
        {
            return CreateKeyboard();
        }

        public override void ProcessMessage(string messageText, TGUser chatId)
        {
            switch (messageText)
            {
                case "расписание на сегодня":
                {
                    messageHandler.GetScheduleForToday(chatId.ToString());
                    break;
                }
                case "расписание на завтра":
                {
                    messageHandler.GetScheduleForNextDay(chatId.ToString());
                    break;
                }
                case "я в столовой":
                {
                    var visitorsCount = messageHandler.GetDinigRoom(chatId.ToString());
                    var text = new MessageResponse(ResponseType.DiningRoom).response;
                    tgMessageSender.SendNotification(chatId, text + visitorsCount, GetKeyboard());
                    break;
                }
                case "ссылки на учебные чаты":
                {
                    messageHandler.GetLinks(chatId.userID.ToString());
                    break;
                }
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                {
                    tgMessageSender.HandleHelpMessage(chatId, GetKeyboard());
                    break;
                }
                default:
                {
                    tgUnknownMessageProcessor.ProcessUnknownCommand(messageText, chatId, GetKeyboard(), new MessageResponse(ResponseType.RegisterError));
                    break;
                }
            }
        }
    }
}