using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Telegram.Bot.Types.ReplyMarkups;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public interface CommandVK
    {
        public MessageKeyboard GetKeyboard();
        public void ProcessMessage(string messageText, BotUser user);
    }
}
