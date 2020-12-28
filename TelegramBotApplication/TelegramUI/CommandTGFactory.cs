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
        private readonly UnknownMessageProcessor unknownMessageProcessor;
        public CommandTGFactory(
            TGMessageSender tgMessageSender,
            IPeopleParser peopleParser,
            MessageHandler messageHandler,
            UnknownMessageProcessor unknownMessageProcessor)
        {
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.messageHandler = messageHandler;
            this.unknownMessageProcessor = unknownMessageProcessor;
        }

        public CommandTG Create(int userState)
        {
            switch (userState)
            {
                case 0:
                    return new NotRegister(tgMessageSender, peopleParser, unknownMessageProcessor);
                case 1:
                    return new RegisterInProcess(messageHandler, tgMessageSender, peopleParser, unknownMessageProcessor);
                case 2:
                    return new Register(messageHandler, tgMessageSender, unknownMessageProcessor);
            }
            throw new Exception("Wrong user state");
        }
    }
}
