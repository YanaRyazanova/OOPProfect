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
        public CommandVKFactory(VKMessageSender vkMessageSender, IPeopleParser peopleParser, MessageHandler messageHandler, VKUnknownMessageProcessor vkUnknownMessageProcessor)
        {
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.messageHandler = messageHandler;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
        }

        public CommandVK Create(int userState)
        {
            return userState switch
            {
                0 => (CommandVK) new NotRegisterVK(vkMessageSender, peopleParser, vkUnknownMessageProcessor),
                1 => new RegisterInProcessVK(messageHandler, vkMessageSender, peopleParser, vkUnknownMessageProcessor),
                2 => new RegisterVK(messageHandler, vkMessageSender, vkUnknownMessageProcessor),
                _ => throw new Exception("Wrong user state")
            };
        }
    }
}
