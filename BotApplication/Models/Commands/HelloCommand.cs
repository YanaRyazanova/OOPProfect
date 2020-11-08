using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotApplication.Models.Commands
{
    public class HelloCommand : Command
    {
        public override string Name => "hello";

        public override void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;

            //TODO: Command logic -_-

            client.SendTextMessageAsync(chatId, "Hello!", replyToMessageId: messageId);
        }
    }
}
