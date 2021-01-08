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
            var container = AddBindings(new StandardKernel());
           
            var senderNotify = container.Get<SenderNotify>();
            var messageHandler = container.Get<MessageHandler>(new ConstructorArgument("senderNotify", senderNotify));
            //var registerProvider =
            //    container.Get<RegisterCommandListProvider>(new ConstructorArgument("messageHandler", messageHandler));
            //var registerInProcessProvider =
            //    container.Get<RegisterInProcessCommandListProvider>(new ConstructorArgument("messageHandler", messageHandler));
            //var notRegister =
            //    container.Get<NotRegicterCommandListProvider>(new ConstructorArgument("messageHandler", messageHandler));
            
            //var telegramBot = GetTelegramBot(container, registerProvider, registerInProcessProvider, notRegister);
            //var vkBot = GetVkBot(container, registerProvider, registerInProcessProvider, notRegister);

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
            container.Bind<IVkMessageSender>().To<VKMessageSender>().InSingletonScope();

            container.Bind<TGUnknownMessageProcessor>().ToSelf();
            container.Bind<VKUnknownMessageProcessor>().ToSelf();


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

        //private static TelegramBotUI GetTelegramBot(
        //    StandardKernel container,
        //    RegisterCommandListProvider registerProvider,
        //    RegisterInProcessCommandListProvider registerInProcessProvider,
        //    NotRegicterCommandListProvider notRegister)
        //{
            
        //    //var tgMessageSender = container.Get<ITGMessageSender>(new ConstructorArgument("client", client));
        //    //var tgUnknownMessageProcessor = container.Get<TGUnknownMessageProcessor>(
        //    //    new ConstructorArgument("tgMessageSender", tgMessageSender),
        //    //    new ConstructorArgument("registerCommandListProvider", registerProvider),
        //    //    new ConstructorArgument("registerInProcessCommandListProvider", registerInProcessProvider),
        //    //    new ConstructorArgument("notRegicterCommandListProvider", notRegister));
        //    //var commandTGFactory = container.Get<CommandTGFactory>(
        //    //    new ConstructorArgument("tgMessageSender", tgMessageSender),
        //    //    new ConstructorArgument("tgUnknownMessageProcessor", tgUnknownMessageProcessor));
        //    //var telegramBot = container.Get<TelegramBotUI>(
        //    //    new ConstructorArgument("newClient", client),
        //    //    new ConstructorArgument("commandTGFactory", commandTGFactory),
        //    //    new ConstructorArgument("tgMessageSender", tgMessageSender));
        //    //return telegramBot;
        //}

        //private static VkBotUI GetVkBot(
        //    StandardKernel container,
        //    RegisterCommandListProvider registerProvider,
        //    RegisterInProcessCommandListProvider registerInProcessProvider,
        //    NotRegicterCommandListProvider notRegister)
        //{
        //    var vkToken = File.ReadAllText("vkToken.txt");
        //    var vkApi = new VkApi();
        //    var vkMessageSender = container.Get<VKMessageSender>(new ConstructorArgument("api", vkApi));
        //    var vkUnknownMessageProcessor = container.Get<VKUnknownMessageProcessor>(
        //        new ConstructorArgument("vkMessageSender", vkMessageSender),
        //        new ConstructorArgument("registerCommandListProvider", registerProvider),
        //        new ConstructorArgument("registerInProcessCommandListProvider", registerInProcessProvider),
        //        new ConstructorArgument("notRegicterCommandListProvider", notRegister));
        //    var commandVKFactory = container.Get<CommandVKFactory>(
        //        new ConstructorArgument("vkMessageSender", vkMessageSender),
        //        new ConstructorArgument("vkUnknownMessageProcessor", vkUnknownMessageProcessor));

        //    var vkBot = container.Get<VkBotUI>(new ConstructorArgument(
        //            "api", vkApi),
        //        new ConstructorArgument("keyVkToken", vkToken),
        //        new ConstructorArgument("vkMessageSender", vkMessageSender),
        //        new ConstructorArgument("commandVkFactory", commandVKFactory));
        //    return vkBot;
        //}
    }
}