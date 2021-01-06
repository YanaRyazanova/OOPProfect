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
            try
            {
                client.SendTextMessageAsync(user.UserId, message, replyMarkup: keyboard).Wait();
            }
            catch (AggregateException)
            {
            }
        }
    }
}
