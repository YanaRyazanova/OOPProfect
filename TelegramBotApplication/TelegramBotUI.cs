using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Application;
using Domain;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace View
{
    public class TelegramBotUI
    {
        private static TelegramBotClient client;
        private static MessageHandler messageHandler;
        //public delegate string MessageHandler(Messages message);
        //public event MessageHandler messageHandlerEvent;
        private static PeopleParser peopleParser;

        public TelegramBotUI(TelegramBotClient newClient, MessageHandler newMessageHandler,
                                                          PeopleParser newPeopleParser)
        {
            client = newClient;
            messageHandler = newMessageHandler;
            //messageHandlerEvent += Application.MessageHandler.GetResponse;
            peopleParser = newPeopleParser;
        }

        public void Run()
        {
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            BotNotificationsSender();
            client.StartReceiving();
        }

        public void Stop()
        {
            client.StopReceiving();
        }

        private static ReplyKeyboardMarkup CreateKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("расписание на сегодня"),
                    new KeyboardButton("расписание на завтра")
                },

                new[]
                {
                    new KeyboardButton("я в столовой"),
                    new KeyboardButton("help")
                }
            });
            return keyboard;
        }

        private static ReplyKeyboardMarkup CreateStartKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("ФТ-201"),
                    new KeyboardButton("ФТ-202")
                }
            });
            return keyboard;
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;
            var messageText = message.Text;
            if (message?.Type != MessageType.Text) return;
            if (messageText == "/keyboard" || messageText == "/start")
            {
                var keyboardStart = CreateStartKeyboard();
                await client.SendTextMessageAsync(chatId, "Добро пожаловать! Мы рады, что вы используете нашего бота! Выберите свою группу:", replyMarkup: keyboardStart);
            }
            else if (messageText == "ФТ-201" || messageText == "ФТ-202")
            {
                peopleParser.AddNewUser(chatId.ToString(), messageText);
                var keyboardMenu = CreateKeyboard();
                await client.SendTextMessageAsync(chatId, "Выберите пункт меню", replyMarkup: keyboardMenu);
            }
            else
            {
                var keyboardMenu = CreateKeyboard();
                await client.SendTextMessageAsync(chatId, "Выберите пункт меню", replyMarkup: keyboardMenu);
                var text = messageHandler.GetResponse(new Messages(messageText, chatId));
                //var text = messageHandlerEvent?.Invoke(new Messages(messageText, chatId));
                await client.SendTextMessageAsync(chatId, text);
            }
        }

        private static async void BotNotificationsSender()
        {
            var usersList = peopleParser.GetAllUsers();
            foreach (var id in usersList)
            {
                var group = peopleParser.GetGroupFromId(id);
                var message = messageHandler.LessonReminderHandler(group);
                if (message == null)
                    continue;
                await client.SendTextMessageAsync(id, message);
            }
        }
    }
}