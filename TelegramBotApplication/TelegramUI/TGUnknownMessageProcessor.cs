using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public class TGUnknownMessageProcessor
    {
        private readonly TGMessageSender tgMessageSender;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private readonly RegisterInProcessCommandListProvider registerInProcessCommandListProvider;
        private readonly NotRegicterCommandListProvider notRegicterCommandListProvider;

        public TGUnknownMessageProcessor(
            TGMessageSender tgMessageSender,
            RegisterCommandListProvider registerCommandListProvider,
            RegisterInProcessCommandListProvider registerInProcessCommandListProvider,
            NotRegicterCommandListProvider notRegicterCommandListProvider)
        {
            this.tgMessageSender = tgMessageSender;
            this.registerCommandListProvider = registerCommandListProvider;
            this.registerInProcessCommandListProvider = registerInProcessCommandListProvider;
            this.notRegicterCommandListProvider = notRegicterCommandListProvider;
        }

        public void ProcessUnknownCommand(string messageText, TGUser chatId, ReplyKeyboardMarkup keyboard, MessageResponse messageResponse)
        {
            var allCommands = registerCommandListProvider.GetCommands()
                .Concat(notRegicterCommandListProvider.GetCommands())
                .Concat(registerInProcessCommandListProvider.GetCommands())
                .ToArray();
            var response = allCommands.Contains(messageText) ? messageResponse : new MessageResponse(ResponseType.Error);
            tgMessageSender.SendNotification(chatId, response.response, keyboard);
        } 
    }
}
