using System;
using System.Collections.Generic;
using System.Text;
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

        public void SendNotification(TGUser chatID, string message, ReplyKeyboardMarkup keyboard)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            
            try
            {
                client.SendTextMessageAsync(chatID.userID, message, replyMarkup: keyboard).Wait();
            }
            catch (AggregateException)
            {
                return;
            }
        }

        public void HandleHelpMessage(TGUser chatId, ReplyKeyboardMarkup keyboard)
        {
            var text = new MessageResponse(ResponseType.Help).response;
            SendNotification(chatId, text, keyboard);
        }
    }
}
