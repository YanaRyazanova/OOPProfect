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

namespace View
{
    class BotApp
    {
        static void Main(string[] args)
        {
            var token = File.ReadAllText("token.txt"); // token, который вернул BotFather
            var container = AddBindings(new StandardKernel());
            
            var client = new TelegramBotClient(token);
            var messageHandler = container.Get<MessageHandler>();
            var bot = container.Get<TelegramBotUI>(new ConstructorArgument("newClient", client),
                new ConstructorArgument("newMessageHandler", messageHandler));
            bot.Run();
            Console.ReadLine();
            bot.Stop();
        }

        private static StandardKernel AddBindings(StandardKernel container)
        {

            container.Bind<TelegramBotUI>().ToSelf();
            container.Bind<DiningRoomIndicator>().ToSelf();
            container.Bind<DataBaseParserSql>().ToSelf();
            container.Bind<DataBaseParserCsv>().ToSelf();
            container.Bind<DataBaseSql>().ToSelf();
            container.Bind<PeopleParserSql>().ToSelf();
            container.Bind<PeopleParserCsv>().ToSelf();
            container.Bind<MessageHandler>().ToSelf();
            container.Bind<LessonReminder>().ToSelf();
            return container;
        }
    }
}