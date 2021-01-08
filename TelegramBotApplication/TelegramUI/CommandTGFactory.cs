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
        private readonly AddingLinkCommandListProvider addingLinkCommandListProvider;

        public CommandTGFactory(
            ITGMessageSender tgMessageSender,
            IPeopleParser peopleParser,
            MessageHandler messageHandler,
            TGUnknownMessageProcessor tgUnknownMessageProcessor,
            GroupProvider groupProvider,
            RegisterCommandListProvider registerCommandListProvider,
            AddingLinkCommandListProvider addingLinkCommandListProvider)
        {
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.messageHandler = messageHandler;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.groupProvider = groupProvider;
            this.registerCommandListProvider = registerCommandListProvider;
            this.addingLinkCommandListProvider = addingLinkCommandListProvider;
        }

        public CommandTG Create(int userState)
        {
            return userState switch
            {
                0 => new NotRegisterTG(tgMessageSender, peopleParser, tgUnknownMessageProcessor, messageHandler, groupProvider, registerCommandListProvider),
                1 => new RegisterInProcessTG(messageHandler, tgMessageSender, peopleParser, tgUnknownMessageProcessor, groupProvider, registerCommandListProvider),
                2 => new RegisterTG(messageHandler, tgMessageSender, tgUnknownMessageProcessor, registerCommandListProvider, peopleParser),
                3 => new AddingLinkTG(messageHandler, peopleParser, tgMessageSender, tgUnknownMessageProcessor, addingLinkCommandListProvider),
                _ => throw new Exception("Wrong user state")
            };
        }
    }
}
