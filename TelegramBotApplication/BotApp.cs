using System;
using System.IO;
using Application;
using Domain.Functions;
using Infrastructure;
using Infrastructure.SQL;
using Ninject;
using Ninject.Parameters;
using Telegram.Bot;
using VkNet;

namespace View
{
    class BotApp
    {
        //private static List<long> usersList = new List<long>();
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
            //var vkBot = container.Get<VkBotUI>(new ConstructorArgument("newVkApi", vkApi),
            //    new ConstructorArgument("newToken", vkToken),
            //    new ConstructorArgument("NewVkMessageHandler", messageHandler));
            var vkBot = new VkBotUI(vkApi, vkToken, messageHandler);
            vkBot.Run();
            telegramBot.Run();
            Console.ReadLine();
            telegramBot.Stop();
        }

        private static StandardKernel AddBindings(StandardKernel container)
        {
            container.Bind<TelegramBotUI>().ToSelf();
            //container.Bind<VkBotUI>().ToSelf();
            container.Bind<MessageHandler>().ToSelf().WithConstructorArgument("groupName", "ФТ-201");
            container.Bind<DiningRoomIndicator>().ToSelf();
            container.Bind<DataBaseParserSql>().ToSelf();
            container.Bind<DataBaseSql>().ToSelf();
            //container.Bind<LessonReminder<DataBaseParser>>().ToSelf();
            return container;
        }
    }
}