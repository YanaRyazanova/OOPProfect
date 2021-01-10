using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;
using Ninject;

namespace View
{
    public class RegisterInProcessTG : CommandTG
    {
        private readonly MessageHandler messageHandler;
        private readonly ITGMessageSender tgMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        private readonly GroupProvider groupProvider;
        private readonly StandardKernel container;

        private ReplyKeyboardMarkup CreateStartKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                groupProvider
                    .GetAllGroups()
                    .Select(x => new KeyboardButton(x))
                    .ToArray()
            });
            return keyboard;
        }

        public RegisterInProcessTG(MessageHandler messageHandler,
            ITGMessageSender tgMessageSender,
            IPeopleParser peopleParser,
            TGUnknownMessageProcessor tgUnknownMessageProcessor,
            GroupProvider groupProvider,
            StandardKernel container)
        {
            this.messageHandler = messageHandler;
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.groupProvider = groupProvider;
            this.container = container;
        }

        public ReplyKeyboardMarkup GetKeyboard()
        {
            return CreateStartKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            switch (messageText)
            {
                default:
                {
                    if (groupProvider.GetAllGroups().Contains(messageText.ToUpper()))
                    {
                        if (messageHandler.SaveGroup(messageText.ToUpper(), user))
                        {
                            peopleParser.EvaluateState(user.UserId);
                            var updatedState = container.Get<RegisterTG>();//new RegisterTG(messageHandler, tgMessageSender, tgUnknownMessageProcessor, registerCommandListProvider, peopleParser, addingLinkCommandListProvider);
                            tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.SuccessfulRegistration).response, updatedState.GetKeyboard());
                        }
                        else
                        {
                            tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.GroupError).response, GetKeyboard());
                        }
                    }
                    else
                    {
                        tgUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.RegisterInProgressError));
                    }
                    break;
                }
            }
        }
    }
}