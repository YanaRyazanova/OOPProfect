using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;

namespace View
{
    public class CommandVKFactory
    {
        private readonly IVkMessageSender vkMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly MessageHandler messageHandler;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;
        private readonly GroupProvider groupProvider;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private readonly AddingLinkCommandListProvider addingLinkCommandListProvider;

        public CommandVKFactory(IVkMessageSender vkMessageSender, 
            IPeopleParser peopleParser, 
            MessageHandler messageHandler, 
            VKUnknownMessageProcessor vkUnknownMessageProcessor,
            GroupProvider groupProvider, 
            RegisterCommandListProvider registerCommandListProvider,
            AddingLinkCommandListProvider addingLinkCommandListProvider)
        {
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.messageHandler = messageHandler;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
            this.groupProvider = groupProvider;
            this.registerCommandListProvider = registerCommandListProvider;
            this.addingLinkCommandListProvider = addingLinkCommandListProvider;
        }

        public CommandVK Create(int userState)
        {
            return userState switch
            {
                0 => new NotRegisterVK(vkMessageSender, peopleParser, messageHandler, vkUnknownMessageProcessor, groupProvider, registerCommandListProvider, addingLinkCommandListProvider),
                1 => new RegisterInProcessVK(messageHandler, vkMessageSender, peopleParser, vkUnknownMessageProcessor, groupProvider, registerCommandListProvider, addingLinkCommandListProvider),
                2 => new RegisterVK(messageHandler, vkMessageSender, vkUnknownMessageProcessor, peopleParser, registerCommandListProvider, addingLinkCommandListProvider),
                3 => new AddingLinkVK(messageHandler, peopleParser, vkMessageSender, addingLinkCommandListProvider, vkUnknownMessageProcessor, registerCommandListProvider),
                _ => throw new Exception("Wrong user state")
            };
        }
    }
}
