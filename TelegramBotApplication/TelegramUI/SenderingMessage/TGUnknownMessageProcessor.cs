﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public class TGUnknownMessageProcessor
    {
        private readonly TGMessageSender tgMessageSender;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private readonly RegisterInProcessCommandListProvider registerInProcessCommandListProvider;
        private readonly NotRegicterCommandListProvider notRegicterCommandListProvider;
        private readonly HelpCommandListProvider helpCommandListProvider;

        public TGUnknownMessageProcessor(
            TGMessageSender tgMessageSender,
            RegisterCommandListProvider registerCommandListProvider,
            RegisterInProcessCommandListProvider registerInProcessCommandListProvider,
            NotRegicterCommandListProvider notRegicterCommandListProvider,
            HelpCommandListProvider helpCommandListProvider)
        {
            this.tgMessageSender = tgMessageSender;
            this.registerCommandListProvider = registerCommandListProvider;
            this.registerInProcessCommandListProvider = registerInProcessCommandListProvider;
            this.notRegicterCommandListProvider = notRegicterCommandListProvider;
            this.helpCommandListProvider = helpCommandListProvider;
        }

        public void ProcessUnknownCommand(string messageText, BotUser user, ReplyKeyboardMarkup keyboard, MessageResponse messageResponse)
        {
            var helpCommands = helpCommandListProvider.GetCommands();
            if (helpCommands.Contains(messageText))
            {
                var helpMessage = new MessageResponse(ResponseType.Help).response;
                tgMessageSender.SendNotification(user, helpMessage, keyboard);
                return;
            }
            var allCommands = registerCommandListProvider.GetCommands()
                .Concat(notRegicterCommandListProvider.GetCommands())
                .Concat(registerInProcessCommandListProvider.GetCommands())
                .Concat(helpCommands)
                .ToArray();
            
            var response = allCommands.Contains(messageText) ? messageResponse : new MessageResponse(ResponseType.Error);
            tgMessageSender.SendNotification(user, response.response, keyboard);
        } 
    }
}
