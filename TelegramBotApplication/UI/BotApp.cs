using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace OOPProject
{
    class BotApp
    {
        private static readonly List<long> usersList = new List<long>();
        private static TelegramBotClient client;
        private static readonly string token = "1443567108:AAEh-njifk9sV2UAASpPJeNF2Jbu8zZ6nUs"; // token, который вернул BotFather
        static void Main(string[] args)
        {
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
            if (message.Type == MessageType.Text)
            {
                await client.SendTextMessageAsync(chatId, message.Text);
            }
        }

        private static async void BotNotificationsSender(object sender, MessageEventArgs messageEventArgs)
        {
            foreach (var id in usersList)
            {
                var message = new Bot.Domain.Functions.LessonReminder("ФТ-201").Do();
                if (message != null)
                    await client.SendTextMessageAsync(id, message);
            }
        }
    }
}