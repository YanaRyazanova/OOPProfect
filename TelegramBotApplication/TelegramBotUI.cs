using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace View
{
    public class TelegramBotUI
    {
        private static TelegramBotClient client;
        private static MessageHandler messageHandler;

        public TelegramBotUI(TelegramBotClient newClient, MessageHandler newMessageHandler)
        {
            client = newClient;
            messageHandler = newMessageHandler;
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

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;
            var messageText = message.Text;
            //if (!usersList.Contains(chatId))
            //    usersList.Add(chatId);
            if (message?.Type != MessageType.Text) return;
            if (messageText == "/keyboard" || messageText == "/start")
            {
                var keyboard = CreateKeyboard();
                await client.SendTextMessageAsync(chatId, "Выберите пункт меню", replyMarkup: keyboard);
            }

            var text = messageHandler.GetResponse(messageText);
            await client.SendTextMessageAsync(chatId, text);
        }

        private static async void BotNotificationsSender()
        {
            //foreach (var id in usersList)
            //{
            //var message = new Domain.Functions.LessonReminder(DateTime.Now, "ФТ-201").Do();
            //await client.SendTextMessageAsync(id, message);
            //}
        }
    }
}