using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        

        static void Main(string[] args)
        {
            var telegramToken = File.ReadAllText("telegramToken.txt"); // token, который вернул BotFather
            var container = AddBindings(new StandardKernel());
            //var vkToken = File.ReadAllText("vkToken.txt");
            //var vkApi = new VkApi();
            var client = new TelegramBotClient(telegramToken);
            var senderNotify = container.Get<SenderNotify>();
            var messageHandler = container.Get<MessageHandler>(new ConstructorArgument("senderNotify", senderNotify));
            var telegramBot = container.Get<TelegramBotUI>(new ConstructorArgument("newClient", client),
                new ConstructorArgument("newMessageHandler", messageHandler));
            //var vkBot = container.Get<VkBotUI>(new ConstructorArgument(
            //        "api", vkApi),
            //    new ConstructorArgument("keyVkToken", vkToken),
            //    new ConstructorArgument("handler", messageHandler));
            senderNotify.OnReply += telegramBot.SendNotificationLesson;
            messageHandler.OnReply += telegramBot.SendNotification;
            //vkBot.Run();
            telegramBot.Run();
            Task.Run(messageHandler.Run);
            //while (true)
            //{
            //    var currentTime = DateTime.Now;
            //    foreach (var time in times)
            //    {
            //        var difference = currentTime.Hour + currentTime.Minute - time.Hour - time.Minute;
            //        if (difference >= 2 || difference < 0) continue;
            //        telegramBot.BotNotificationsSender();
            //        vkBot.BotNotificationSender();
            //    }
            //}
            Console.ReadLine();
            //telegramBot.Stop();
        }

        private static StandardKernel AddBindings(StandardKernel container)
        {

            container.Bind<TelegramBotUI>().ToSelf();
            container.Bind<VkBotUI>().ToSelf();
            container.Bind<DiningRoomIndicator>().ToSelf();
            container.Bind<SenderNotify>().ToSelf();
            container.Bind<MessageHandler>().ToSelf();
            container.Bind<LessonReminder>().ToSelf();

            //container.Bind<DataBaseSql>().ToSelf();
            
            container.Bind<IDataBaseParser>().To<DataBaseParserSql>();
            container.Bind<IPeopleParser>().To<PeopleParserSql>();
            return container;
        }
    }
}