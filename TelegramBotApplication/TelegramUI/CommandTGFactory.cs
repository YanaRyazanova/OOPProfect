﻿using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using View.TelegramUI.UserStates;

namespace View
{
    public class CommandTGFactory
    {
        private readonly ITGMessageSender tgMessageSender;

        private readonly IPeopleParser peopleParser;
        private readonly MessageHandler messageHandler;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        private readonly GroupProvider groupProvider;
        private readonly RegisterCommandListProvider registerCommandListProvider;

        public CommandTGFactory(
            ITGMessageSender tgMessageSender,
            IPeopleParser peopleParser,
            MessageHandler messageHandler,
            TGUnknownMessageProcessor tgUnknownMessageProcessor,
            GroupProvider groupProvider,
            RegisterCommandListProvider registerCommandListProvider)
        {
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.messageHandler = messageHandler;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.groupProvider = groupProvider;
            this.registerCommandListProvider = registerCommandListProvider;
        }

        public CommandTG Create(int userState)
        {
            return userState switch
            {
                0 => (CommandTG) new NotRegisterTG(tgMessageSender, peopleParser, tgUnknownMessageProcessor, messageHandler, groupProvider, registerCommandListProvider),
                1 => new RegisterInProcessTG(messageHandler, tgMessageSender, peopleParser, tgUnknownMessageProcessor, groupProvider, registerCommandListProvider),
                2 => new RegisterTG(messageHandler, tgMessageSender, tgUnknownMessageProcessor, registerCommandListProvider),
                3 => new AddingLink(messageHandler, peopleParser, tgMessageSender),
                _ => throw new Exception("Wrong user state")
            };
        }
    }
}
