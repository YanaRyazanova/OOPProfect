using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Model.Keyboard;

namespace View
{
    public interface IVkMessageSender
    {
        public void SendNotification(VKUser userID, string message, MessageKeyboard keyboard);
        public void HandleHelpMessage(VKUser userID, MessageKeyboard keyboard);
    }
}
