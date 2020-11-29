using System;
using Application;
using Domain.Functions;
using Infrastructure;
using Ninject;
using Ninject.Parameters;
using Telegram.Bot;

namespace View
{
    class BotApp
    {
        //private static List<long> usersList = new List<long>();
        static void Main(string[] args)
        {
            var token = "1443567108:AAEh-njifk9sV2UAASpPJeNF2Jbu8zZ6nUs"; // token, который вернул BotFather
            var container = AddBindings(new StandardKernel());
            
            //var bot = new TelegramBotUI(client);
            var client = new TelegramBotClient(token);
            var bot = container.Get<TelegramBotUI>(new ConstructorArgument("newClient", client));
            bot.Run();
            Console.ReadLine();
            bot.Stop();
        }

        private static StandardKernel AddBindings(StandardKernel container)
        {
            container.Bind<TelegramBotUI>().ToSelf();
            container.Bind<MessageHandler>().ToSelf();
            container.Bind<LessonReminder<Lesson>>().ToSelf();
            container.Bind<DiningRoomIndicator>().ToSelf();
            container.Bind<ScheduleSender<Lesson>>().ToSelf();
            container.Bind<DataBaseParser>().ToSelf();
            container.Bind<DataBase>().ToSelf();
            return container;
        }
    }
}