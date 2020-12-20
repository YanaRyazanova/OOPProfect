using System;
using System.IO;
using Application;
using Domain.Functions;
using Infrastructure;
using Infrastructure.Csv;
using Infrastructure.SQL;
using Ninject;
using Ninject.Parameters;
using Telegram.Bot;
using VkNet;

namespace View
{
    class BotApp
    {
        static void Main(string[] args)
        {
            //var token = File.ReadAllText("token.txt"); // token, который вернул BotFather
            var telegramToken = File.ReadAllText("telegramToken.txt"); // token, который вернул BotFather
            var container = AddBindings(new StandardKernel());
            var vkToken = File.ReadAllText("vkToken.txt");
            var vkApi = new VkApi();
            //var vkApi = container.Get<VkApi>();
            var client = new TelegramBotClient(telegramToken);
            var messageHandler = container.Get<MessageHandler>();
            var telegramBot = container.Get<TelegramBotUI>(new ConstructorArgument("newClient", client),
                new ConstructorArgument("newMessageHandler", messageHandler));
            var vkBot = new VkBotUI(vkApi, vkToken, messageHandler);
            //var vkBot = container.Get<VkBotUI>(new ConstructorArgument(
                    //"newVkApi", vkApi), 
                //new ConstructorArgument("newToken", vkToken),
               // new ConstructorArgument("NewVkMessageHandler", messageHandler));
            vkBot.Run();
            telegramBot.Run();
            Console.ReadLine();
            telegramBot.Stop();
        }

        private static StandardKernel AddBindings(StandardKernel container)
        {

            container.Bind<TelegramBotUI>().ToSelf();
            container.Bind<VkApi>().ToSelf();
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