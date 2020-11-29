using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Application;
using Infrastructure;

namespace View
{
    public class TelegramBotUI
    {
        private static TelegramBotClient client;
        private static  MessageHandler messageHandler;

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

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;
            //if (!usersList.Contains(chatId))
            //    usersList.Add(chatId);
            if (message?.Type == MessageType.Text)
            {
                var text = messageHandler.GetResponse(message.Text);
                await client.SendTextMessageAsync(chatId, text);
            }
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