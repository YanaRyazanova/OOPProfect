using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public class TGMessageSender : ITGMessageSender
    {
        private readonly TelegramBotClient client;

        public TGMessageSender(TelegramBotClient client)
        {
            this.client = client;
        }

        public void SendNotification(BotUser user, string message, ReplyKeyboardMarkup keyboard)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            
            try
            {
                client.SendTextMessageAsync(user.UserId, message, replyMarkup: keyboard).Wait();
            }
            catch (AggregateException)
            {
                return;
            }
        }

        public void HandleHelpMessage(BotUser user, ReplyKeyboardMarkup keyboard)
        {
            var text = new MessageResponse(ResponseType.Help).response;
            SendNotification(user, text, keyboard);
        }
    }
}
