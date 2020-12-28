using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;

namespace View
{
    public class CommandTGFactory
    {
        private readonly TGMessageSender tgMessageSender;

        private readonly IPeopleParser peopleParser;
        private readonly MessageHandler messageHandler;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        public CommandTGFactory(TGMessageSender tgMessageSender, IPeopleParser peopleParser, MessageHandler messageHandler, TGUnknownMessageProcessor tgUnknownMessageProcessor)
        {
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.messageHandler = messageHandler;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
        }

        public CommandTG Create(int userState)
        {
            return userState switch
            {
                0 => (CommandTG) new NotRegisterTG(tgMessageSender, peopleParser, tgUnknownMessageProcessor),
                1 => new RegisterInProcessTG(messageHandler, tgMessageSender, peopleParser, tgUnknownMessageProcessor),
                2 => new RegisterTG(messageHandler, tgMessageSender, tgUnknownMessageProcessor),
                _ => throw new Exception("Wrong user state")
            };
        }
    }
}
