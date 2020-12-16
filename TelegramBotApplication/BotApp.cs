using System;
using System.Collections.Generic;
using System.IO;
using Application;
using Domain.Functions;
using Infrastructure;
using Infrastructure.Csv;
using Infrastructure.SQL;
using Ninject;
using Ninject.Parameters;
using NUnit.Framework;
using Telegram.Bot;
using VkNet;

namespace View
{
    class BotApp
    {
        private static List<DateTime> times = new List<DateTime>
        {
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 50, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 30, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 40, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 20, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 00, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 40, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0)
        };

        static void Main(string[] args)
        {
            var telegramToken = File.ReadAllText("telegramToken.txt"); // token, который вернул BotFather
            var container = AddBindings(new StandardKernel());
            var vkToken = File.ReadAllText("vkToken.txt");
            var vkApi = new VkApi();
            var client = new TelegramBotClient(telegramToken);
            var messageHandler = container.Get<MessageHandler>();
            var telegramBot = container.Get<TelegramBotUI>(new ConstructorArgument("newClient", client),
                new ConstructorArgument("newMessageHandler", messageHandler));
            var vkBot = container.Get<VkBotUI>(new ConstructorArgument(
                    "api", vkApi),
                new ConstructorArgument("keyVkToken", vkToken),
                new ConstructorArgument("handler", messageHandler));
            messageHandler.OnReply += telegramBot.SendNotification;
            vkBot.Run();
            telegramBot.Run();
            while (true)
            {
                var currentTime = DateTime.Now;
                foreach (var time in times)
                {
                    var difference = currentTime.Hour + currentTime.Minute - time.Hour - time.Minute;
                    if (difference >= 2 || difference < 0) continue;
                    telegramBot.BotNotificationsSender();
                    vkBot.BotNotificationSender();
                }
            }
            //Console.ReadLine();
            //telegramBot.Stop();
        }

        private static StandardKernel AddBindings(StandardKernel container)
        {

            container.Bind<TelegramBotUI>().ToSelf();
            container.Bind<VkBotUI>().ToSelf();

            container.Bind<DiningRoomIndicator>().ToSelf();

            container.Bind<DataBaseParserCsv>().ToSelf();
            container.Bind<DataBaseSql>().ToSelf();
            container.Bind<PeopleParserSql>().ToSelf();
            container.Bind<PeopleParserCsv>().ToSelf();
            container.Bind<MessageHandler>().ToSelf();
            container.Bind<LessonReminder>().ToSelf();
            container.Bind<DataBaseParserSql>().ToSelf();
            container.Bind<DataBaseSql>().ToSelf();
            return container;
        }
    }
}