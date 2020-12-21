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
        
        private ReplyKeyboardMarkup keyboardStart;
        private ReplyKeyboardMarkup keyboardMenu;

        public TelegramBotUI(TelegramBotClient newClient, MessageHandler newMessageHandler,
                                                          PeopleParserSql newPeopleParserSql,
                                                          PeopleParserCsv newPeopleParserCsv)
        {
            client = newClient;
            messageHandler = newMessageHandler;
            peopleParserSql = newPeopleParserSql;
            peopleParserCsv = newPeopleParserCsv;
            keyboardStart = CreateStartKeyboard();
            keyboardMenu = CreateKeyboard();
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
            var messageText = message.Text.ToLower();
            try
            {
                
                if (message?.Type != MessageType.Text) return;
                switch (messageText)
                {
                    case "/start":
                    {
                        var text = new MessageResponse(ResponseType.Start).response;
                        SendSubsidiaryNotification(chatId, text, keyboardStart);
                        break;
                    }
                    case "help":
                    case "/help":
                    case "помощь":
                    case "помоги":
                    {
                        var text = new MessageResponse(ResponseType.Help).response;
                        SendSubsidiaryNotification(chatId, text, keyboardMenu);
                        break;
                    }
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
                        messageHandler.GetDinigRoom(chatId.ToString());
                        break;
                    }
                    case "фт-201":
                    case "фт-202":
                    {
                        messageHandler.GetGroup(messageText.ToUpper(), chatId);
                        SendSubsidiaryNotification(chatId, "Выберите пункт меню", keyboardMenu);
                        break;
                    }
                    default:
                    {
                        var text = new MessageResponse(ResponseType.Error).response;
                        SendNotification(chatId.ToString(), text);
                       // messageHandler.GetResponse(new MessageRequest(messageText, chatId));
                            //await client.SendTextMessageAsync(chatId, text, replyMarkup: keyboardMenu);
                        break;
                    }
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

        public async void SendNotification(string chatID, string message)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            await client.SendTextMessageAsync(chatID, message, replyMarkup: keyboardMenu);
        }

        private async void SendSubsidiaryNotification(long chatID, string text, ReplyKeyboardMarkup keyboard)
        {
            await client.SendTextMessageAsync(chatID, text, replyMarkup: keyboard);
        }

        public async void SendNotificationLesson(string chatID, string message)
        {
            await client.SendTextMessageAsync(chatID, message, replyMarkup: keyboardMenu);
        }
    }
}