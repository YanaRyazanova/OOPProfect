using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Domain.Functions;

namespace OOPProject
{
    class BotApp
    {
        private static List<long> usersList = new List<long>();
        private static TelegramBotClient client;
        static void Main(string[] args)
        {
            var token = "1443567108:AAEh-njifk9sV2UAASpPJeNF2Jbu8zZ6nUs"; // token, который вернул BotFather
            client = new TelegramBotClient(token);
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.OnMessage += BotNotificationsSender;
            client.OnMessageEdited += BotNotificationsSender;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;
            if (!usersList.Contains(chatId))
                usersList.Add(chatId);
            if (message?.Type == MessageType.Text)
            {
                await client.SendTextMessageAsync(chatId, message.Text);
            }

            //foreach (var id in usersList)
            //{
            //    var notification = Domain.Functions.LessonReminder.Do(DateTime.Now.AddMinutes(11));
            //    await client.SendTextMessageAsync(id, notification);
            //}
        }

        private static async void BotNotificationsSender(object sender, MessageEventArgs messageEventArgs)
        {
            foreach (var id in usersList)
            {
                //var message = new LessonReminder("ФТ-201").Do();
                //await client.SendTextMessageAsync(id, message);
            }
        }
    }
}