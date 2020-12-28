using System;
using System.Collections.Generic;
using System.Text;
using Application;
using VkNet.Model.Keyboard;

namespace View
{
    public interface IVkMessageSender
    {
        public void SendNotification(BotUser user, string message, MessageKeyboard keyboard);
        public void HandleHelpMessage(BotUser user, MessageKeyboard keyboard);
    }
}
