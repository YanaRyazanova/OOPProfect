using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;

namespace View
{
    public class CommandVKFactory
    {
        private readonly VKMessageSender vkMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly MessageHandler messageHandler;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;
        private readonly GroupProvider groupProvider;

        public CommandVKFactory(VKMessageSender vkMessageSender, 
            IPeopleParser peopleParser, 
            MessageHandler messageHandler, 
            VKUnknownMessageProcessor vkUnknownMessageProcessor,
            GroupProvider groupProvider)
        {
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.messageHandler = messageHandler;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
            this.groupProvider = groupProvider;
        }

        public CommandVK Create(int userState)
        {
            return userState switch
            {
                0 => (CommandVK) new NotRegisterVK(vkMessageSender, peopleParser, messageHandler, vkUnknownMessageProcessor, groupProvider),
                1 => new RegisterInProcessVK(messageHandler, vkMessageSender, peopleParser, vkUnknownMessageProcessor, groupProvider),
                2 => new RegisterVK(messageHandler, vkMessageSender, vkUnknownMessageProcessor),
                _ => throw new Exception("Wrong user state")
            };
        }
    }
}
