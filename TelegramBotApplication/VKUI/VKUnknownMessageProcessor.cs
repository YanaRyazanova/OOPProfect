using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using View.VKUI;
using VkNet.Model.Keyboard;

namespace View
{
    public class VKUnknownMessageProcessor
    {
        private readonly VKMessageSender vkMessageSender;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private readonly RegisterInProcessCommandListProvider registerInProcessCommandListProvider;
        private readonly NotRegicterCommandListProvider notRegicterCommandListProvider;

        public VKUnknownMessageProcessor(
            VKMessageSender vkMessageSender,
            RegisterCommandListProvider registerCommandListProvider,
            RegisterInProcessCommandListProvider registerInProcessCommandListProvider,
            NotRegicterCommandListProvider notRegicterCommandListProvider)
        {
            this.vkMessageSender = vkMessageSender;
            this.registerCommandListProvider = registerCommandListProvider;
            this.registerInProcessCommandListProvider = registerInProcessCommandListProvider;
            this.notRegicterCommandListProvider = notRegicterCommandListProvider;
        }

        public void ProcessUnknownCommand(string messageText, VKUser userId, MessageKeyboard keyboard, MessageResponse messageResponse)
        {
            var allCommands = registerCommandListProvider.GetCommands()
                .Concat(notRegicterCommandListProvider.GetCommands())
                .Concat(registerInProcessCommandListProvider.GetCommands())
                .ToArray();
            var response = allCommands.Contains(messageText) ? messageResponse : new MessageResponse(ResponseType.Error);
            vkMessageSender.SendNotification(userId, response.response, keyboard);
        }
    }
}
