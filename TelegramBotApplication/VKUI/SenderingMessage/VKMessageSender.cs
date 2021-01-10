using System;
using Application;
using VkNet;
using VkNet.Exception;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace View
{
    public class VKMessageSender 
    {
        private readonly VkApi api;
        private readonly Random rnd = new Random();

        public VKMessageSender(VkApi api)
        {
            this.api = api;
        }

        public void SendNotification(BotUser user, string message, MessageKeyboard keyboard)
        {
            if (user.Domain != "vk")
                return;
            api.Messages.Send(new MessagesSendParams
            {
                UserId = long.Parse(user.UserId),
                Message = message,
                RandomId = rnd.Next(),
                Keyboard = keyboard
            });
        }
    }
}
