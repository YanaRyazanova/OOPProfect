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
            var vkToken = File.ReadAllText("vkToken.txt");
            var vkApi = new VkApi();
            var client = new TelegramBotClient(telegramToken);
            var senderNotify = container.Get<SenderNotify>();
            var tgMessageSender = container.Get<TGMessageSender>(new ConstructorArgument("client", client));
            var messageHandler = container.Get<MessageHandler>(new ConstructorArgument("senderNotify", senderNotify));
            var telegramBot = container.Get<TelegramBotUI>(
                new ConstructorArgument("newClient", client),
                new ConstructorArgument("tgItgMessageSender", tgMessageSender));
            //var vkBot = container.Get<VkBotUI>(new ConstructorArgument(
            //        "api", vkApi),
            //    new ConstructorArgument("keyVkToken", vkToken),
            //    new ConstructorArgument("handler", messageHandler));

            senderNotify.OnReply += telegramBot.SendNotificationLesson;
            messageHandler.OnReply += telegramBot.SendNotificationLesson;
            //senderNotify.OnReplyVK += vkBot.SendMessage;
            //messageHandler.OnReplyVK += vkBot.SendMessage;
            //vkBot.Run();
            telegramBot.Run();
            Task.Run(messageHandler.Run);
            Console.ReadLine();
        }

        private static StandardKernel AddBindings(StandardKernel container)
        {
            container.Bind<TelegramBotUI>().ToSelf();
            container.Bind<VkBotUI>().ToSelf();

            container.Bind<ITGMessageSender>().To<TGMessageSender>().InSingletonScope();
            container.Bind<CommandTGFactory>().ToSelf();

            container.Bind<DiningRoomIndicator>().ToSelf();
            container.Bind<SenderNotify>().ToSelf();
            container.Bind<MessageHandler>().ToSelf();
            container.Bind<LessonReminder>().ToSelf();

            container.Bind<IDataBaseParser>().To<DataBaseParserSql>();
            container.Bind<IPeopleParser>().To<PeopleParserSql>();
            container.Bind<ILinkParser>().To<LinkParserSQL>();
            return container;
        }
    }
}