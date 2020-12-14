using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Application;
using Domain;
using Infrastructure;
using Infrastructure.SQL;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace View
{
    public class TelegramBotUI
    {
        private static TelegramBotClient client;
        private static MessageHandler messageHandler;
        private static PeopleParserSQL peopleParserSql;
        private Dictionary<string, DateTime> usersLastNotify = new Dictionary<string, DateTime>();

        public TelegramBotUI(TelegramBotClient newClient, MessageHandler newMessageHandler,
                                                          PeopleParserSQL newPeopleParserSql)
        {
            client = newClient;
            messageHandler = newMessageHandler;
            peopleParserSql = newPeopleParserSql;
        }

        public void Run()
        {
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            //BotNotificationsSender();
            //Task.Run(BotNotificationsSender);
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
            Console.WriteLine(chatId);
            var messageText = message.Text;
            if (message?.Type != MessageType.Text) return;
            if (messageText == "/keyboard" || messageText == "/start")
            {
                var keyboardStart = CreateStartKeyboard();
                await client.SendTextMessageAsync(chatId, "Добро пожаловать! Мы рады, что вы используете нашего бота! Выберите свою группу:", replyMarkup: keyboardStart);
            }
            else if (messageText == "ФТ-201" || messageText == "ФТ-202")
            {
                peopleParserSql.AddNewUser(chatId.ToString(), messageText);
                var keyboardMenu = CreateKeyboard();
                await client.SendTextMessageAsync(chatId, "Выберите пункт меню", replyMarkup: keyboardMenu);
            }
            else
            {
                var keyboardMenu = CreateKeyboard();
                var text = messageHandler.GetResponse(new MessageRequest(messageText, chatId));
                //var text = messageHandlerEvent?.Invoke(new Messages(messageText, chatId));
                await client.SendTextMessageAsync(chatId, text, replyMarkup: keyboardMenu);
                //await Task.Run(BotNotificationsSender);
            }
        }

        private async void BotNotificationsSender()
        {
            Console.WriteLine("Hello from BotNotificationSender");
            var usersList = peopleParserSql.GetAllUsers();
            foreach (var id in usersList)
            { 
                var flag = false;
                if (!usersLastNotify.ContainsKey(id))
                {
                    usersLastNotify[id] = DateTime.Now;
                    flag = true;
                }
                var group = peopleParserSql.GetGroupFromId(id);
                var message = messageHandler.LessonReminderHandler(group);
                if (message == null || (DateTime.Now.Minute - usersLastNotify[id].Minute < 87  && !flag))
                    continue;
                if (message.Contains("пар больше нет")) //!!!!!
                    continue;
                try
                {
                    usersLastNotify[id] = DateTime.Now;
                    await client.SendTextMessageAsync(id, message);
                }
                catch
                {
                    return;
                }
            }
        }
    }
}