using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public interface ITGMessageSender
    {
        public void SendNotification(TGUser chatID, string message, ReplyKeyboardMarkup keyboard);
        public void HandleHelpMessage(TGUser chatId, ReplyKeyboardMarkup keyboard);
    }
}
