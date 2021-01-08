using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;

namespace View.TelegramUI.UserStates
{
    public class AddingLinkTG : CommandTG
    {
        private readonly MessageHandler messageHandler;
        private readonly IPeopleParser peopleParser;
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

        public ReplyKeyboardMarkup GetKeyboard()
        {
            return CreateKeyboard();
        }

        public AddingLinkTG(MessageHandler messageHandler,
            IPeopleParser peopleParser,
            ITGMessageSender tgMessageSender,
            TGUnknownMessageProcessor tgUnknownMessageProcessor)
        {
            this.messageHandler = messageHandler;
            this.peopleParser = peopleParser;
            this.tgMessageSender = tgMessageSender;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
        }

        public void ProcessMessage(string messageText, BotUser user)
        {

            if (!messageText.Contains("http") && !messageText.Contains(":")) 
                tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinksError).response, GetKeyboard());
            var splittedMessage = messageText.Split(": ");
            var name = splittedMessage[0];
            var link = splittedMessage[1];
            messageHandler.AddLink(user, name, link);
            tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.SucessfulLinks).response, GetKeyboard());
            peopleParser.ChangeState(user.UserId, "2");
        }
    }
}
