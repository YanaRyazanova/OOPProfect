using System;
using System.Collections.Generic;
using System.Text;
using VkNet;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace View.VKUI
{
    public class VKMessageSender : IVkMessageSender
    {
        private readonly VkApi api;
        private readonly Random rnd = new Random();

        public VKMessageSender(VkApi api)
        {
            this.api = api;
        }

        public void SendNotification(VKUser userID, string message, MessageKeyboard keyboard)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            try
            {
                api.Messages.Send(new MessagesSendParams
                {
                    UserId = userID.userID,
                    Message = message,
                    RandomId = rnd.Next(),
                    Keyboard = keyboard
                });
            }
            catch (AggregateException)
            {
                return;
            }
        }

        public void HandleHelpMessage(VKUser userID, MessageKeyboard keyboard)
        {
            var text = new MessageResponse(ResponseType.Help).response;
            SendNotification(userID, text, keyboard);
        }
    }
}
