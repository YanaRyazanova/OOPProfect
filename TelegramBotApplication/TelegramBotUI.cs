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
using Infrastructure.Csv;
using Infrastructure.SQL;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace View
{
    public class TelegramBotUI
    {
        private static TelegramBotClient client;
        private static MessageHandler messageHandler;
        private static PeopleParserSql peopleParserSql;
        private static PeopleParserCsv peopleParserCsv;
        private Dictionary<string, DateTime> usersLastNotify = new Dictionary<string, DateTime>();

        public TelegramBotUI(TelegramBotClient newClient, MessageHandler newMessageHandler,
                                                          PeopleParserSql newPeopleParserSql,
                                                          PeopleParserCsv newPeopleParserCsv)
        {
            client = newClient;
            messageHandler = newMessageHandler;
            peopleParserSql = newPeopleParserSql;
            peopleParserCsv = newPeopleParserCsv;
        }

        public void Run()
        {
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
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
                    new KeyboardButton("Расписание на сегодня"),
                    new KeyboardButton("Расписание на завтра")
                },

                new[]
                {
                    new KeyboardButton("Я в столовой"),
                    new KeyboardButton("Help")
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
            try
            {
                if (message?.Type != MessageType.Text) return;
                if (messageText == "/keyboard" || messageText == "/start")
                {
                    var keyboardStart = CreateStartKeyboard();
                    var text = new MessageResponse(ResponseType.Start).response;
                    await client.SendTextMessageAsync(chatId, text, replyMarkup: keyboardStart);
                }
                else if (messageText == "ФТ-201" || messageText == "ФТ-202")
                {
                    peopleParserSql.AddNewUser(chatId.ToString(), messageText);
                    //peopleParserCsv.AddNewUser(chatId.ToString(), messageText);
                    var keyboardMenu = CreateKeyboard();
                    await client.SendTextMessageAsync(chatId, "Выберите пункт меню", replyMarkup: keyboardMenu);
                }
                else
                {
                    var keyboardMenu = CreateKeyboard();
                    var text = messageHandler.GetResponse(new MessageRequest(messageText.ToLower(), chatId));
                    await client.SendTextMessageAsync(chatId, text, replyMarkup: keyboardMenu);
                    //await Task.Run(BotNotificationsSender);
                }
            }
            catch (Exception e)
            {
                if (e.Message == "constraint failed\r\nUNIQUE constraint failed: PeopleAndGroups.ChatID")
                {
                    var text = "Вы уже зарегистрированы в боте. Смену группы мы добавим позже :-)";
                    await client.SendTextMessageAsync(chatId, text, replyMarkup: CreateKeyboard());
                }
                else
                {
                    var text = "Упс! Кажется что-то пошло не так!Попробуйте начать с команды '/start'";
                    Console.WriteLine(e);
                    await client.SendTextMessageAsync(chatId, text, replyMarkup: CreateStartKeyboard());
                }
            }
        }

        public async void BotNotificationsSender()
        {
            Console.WriteLine("Hello from BotNotification");
            var usersList = peopleParserSql.GetAllUsers();
            //var usersList = peopleParserCsv.GetAllUsers();
            foreach (var id in usersList)
            { 
                var flag = false;
                if (!usersLastNotify.ContainsKey(id))
                {
                    usersLastNotify[id] = DateTime.Now;
                    flag = true;
                }
                var group = peopleParserSql.GetGroupFromId(id);
                //var group = peopleParserCsv.GetGroupFromId(id);
                var message = messageHandler.LessonReminderHandler(group);
                if (message == null || (DateTime.Now.Minute - usersLastNotify[id].Minute < //87
                                                                                           1 && !flag))
                    continue;
                if (message.Contains("пар больше нет"))
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