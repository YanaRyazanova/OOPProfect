using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;
using View.TelegramUI.UserStates;
using Ninject;

namespace View
{
    public class RegisterTG : CommandTG
    {
        private readonly StandardKernel container;
        private readonly MessageHandler messageHandler;
        private readonly TGMessageSender tgMessageSender;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private readonly IPeopleParser peopleParser;

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

        public RegisterTG(StandardKernel container,
            MessageHandler messageHandler,
            TGMessageSender tgMessageSender,
            TGUnknownMessageProcessor tgUnknownMessageProcessor,
            RegisterCommandListProvider registerCommandListProvider,
            IPeopleParser peopleParser)
        {
            this.container = container;
            this.messageHandler = messageHandler;
            this.tgMessageSender = tgMessageSender;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.registerCommandListProvider = registerCommandListProvider;
            this.peopleParser = peopleParser;
        }

        public ReplyKeyboardMarkup GetKeyboard()
        {
            return CreateKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
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
                case var n when n == registerCommandListProvider.GettingDiningRoom:
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
                    peopleParser.ChangeState(user.UserId, "3");
                    var newUserState = container.Get<AddingLinkTG>();
                    tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinksMessage).response, newUserState.GetKeyboard());
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