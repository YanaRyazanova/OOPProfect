using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application;
using VkNet.Model.Keyboard;

namespace View
{
    public class VKUnknownMessageProcessor
    {
        private readonly VKMessageSender vkMessageSender;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private readonly RegisterInProcessCommandListProvider registerInProcessCommandListProvider;
        private readonly NotRegicterCommandListProvider notRegicterCommandListProvider;
        private readonly HelpCommandListProvider helpCommandListProvider;

        public VKUnknownMessageProcessor(
            VKMessageSender vkMessageSender,
            RegisterCommandListProvider registerCommandListProvider,
            RegisterInProcessCommandListProvider registerInProcessCommandListProvider,
            NotRegicterCommandListProvider notRegicterCommandListProvider,
            HelpCommandListProvider helpCommandListProvider)
        {
            this.vkMessageSender = vkMessageSender;
            this.registerCommandListProvider = registerCommandListProvider;
            this.registerInProcessCommandListProvider = registerInProcessCommandListProvider;
            this.notRegicterCommandListProvider = notRegicterCommandListProvider;
            this.helpCommandListProvider = helpCommandListProvider;
        }

        public void ProcessUnknownCommand(string messageText, BotUser user, MessageKeyboard keyboard, MessageResponse messageResponse)
        {
            var helpCommands = helpCommandListProvider.GetCommands();
            if (helpCommands.Contains(messageText))
            {
                var helpMessage = new MessageResponse(ResponseType.Help).response;
                vkMessageSender.SendNotification(user, helpMessage, keyboard);
                return;
            }

            var allCommands = registerCommandListProvider.GetCommands()
                .Concat(notRegicterCommandListProvider.GetCommands())
                .Concat(registerInProcessCommandListProvider.GetCommands())
                .Concat(helpCommands)
                .ToArray();
            var response = allCommands.Contains(messageText) ? messageResponse : new MessageResponse(ResponseType.Error);
            vkMessageSender.SendNotification(user, response.response, keyboard);
        }
    }
}
