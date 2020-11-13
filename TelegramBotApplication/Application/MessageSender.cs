using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace BotApp
{
    class MessageSender
    {
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;
            if (!usersList.Contains(chatId))
                usersList.Add(chatId);
            if (message.Type == MessageType.Text)
            {
                await client.SendTextMessageAsync(chatId, message.Text);
            }
        }
    }
}
