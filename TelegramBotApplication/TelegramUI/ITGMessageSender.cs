using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public interface ITGMessageSender
    {
        public void SendNotification(BotUser user, string message, ReplyKeyboardMarkup keyboard);
        public void HandleHelpMessage(BotUser user, ReplyKeyboardMarkup keyboard);
    }
}
