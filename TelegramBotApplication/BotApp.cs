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
using View.TelegramUI.UserStates;
using VkNet;

namespace View
{
    class BotApp
    {
        static void Main(string[] args)
        {
            var container = AddBindings(new StandardKernel());
            container.Bind<StandardKernel>().ToConstant(container);

            var senderNotify = container.Get<SenderNotify>();
            var messageHandler = container.Get<MessageHandler>(new ConstructorArgument("senderNotify", senderNotify));
            var telegramBot = container.Get<TelegramBotUI>();

            var vkToken = File.ReadAllText("vkToken.txt");
            var vkBot = container.Get<VkBotUI>(new ConstructorArgument("keyVkToken", vkToken));

            messageHandler.OnReply += telegramBot.SendMessage;
            messageHandler.OnReply += vkBot.SendMessage;

            vkBot.Run();
            telegramBot.Run();
            Task.Run(messageHandler.Run);
            Console.ReadLine();
        }

        private static StandardKernel AddBindings(StandardKernel container)
        {
            var telegramToken = File.ReadAllText("telegramToken.txt"); // token, который вернул BotFather
            container.Bind<TelegramBotClient>().ToConstant(new TelegramBotClient(telegramToken));
            container.Bind<VkApi>().ToConstant(new VkApi());

            container.Bind<TelegramBotUI>().ToSelf();
            container.Bind<VkBotUI>().ToSelf();

            container.Bind<ITGMessageSender>().To<TGMessageSender>().InSingletonScope();
            container.Bind<VKMessageSender>().ToSelf().InSingletonScope();

            container.Bind<TGUnknownMessageProcessor>().ToSelf();
            container.Bind<VKUnknownMessageProcessor>().ToSelf();

            container.Bind<AddingLinkVK>().ToSelf();
            container.Bind<NotRegisterVK>().ToSelf();
            container.Bind<RegisterVK>().ToSelf();
            container.Bind<RegisterInProcessVK>().ToSelf();

            container.Bind<AddingLinkTG>().ToSelf();
            container.Bind<NotRegisterTG>().ToSelf();
            container.Bind<RegisterTG>().ToSelf();
            container.Bind<RegisterInProcessTG>().ToSelf();


            container.Bind<DiningRoomIndicator>().ToSelf().InSingletonScope();
            container.Bind<SenderNotify>().ToSelf().InSingletonScope();
            container.Bind<MessageHandler>().ToSelf().InSingletonScope();
            container.Bind<LessonReminder>().ToSelf();
            container.Bind<GroupProvider>().ToSelf();

            container.Bind<CommandTGFactory>().ToSelf().InSingletonScope();
            container.Bind<CommandVKFactory>().ToSelf().InSingletonScope();

            container.Bind<IDataBaseParser>().To<DataBaseParserSql>();
            container.Bind<IPeopleParser>().To<PeopleParserSql>();
            container.Bind<ILinkParser>().To<LinkParserSQL>();
            return container;
        }
    }
}