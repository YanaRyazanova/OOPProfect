using System;
using System.Collections.Generic;
using System.Text;
using Application;
using VkNet;
using VkNet.Exception;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace View
{
    public class VKMessageSender : IVkMessageSender
    {
        private readonly VkApi api;
        private readonly Random rnd = new Random();

        public VKMessageSender(VkApi api)
        {
            this.api = api;
        }

        public void SendNotification(BotUser user, string message, MessageKeyboard keyboard)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            try
            {
                api.Messages.Send(new MessagesSendParams
                {
                    UserId = long.Parse(user.UserId),
                    Message = message,
                    RandomId = rnd.Next(),
                    Keyboard = keyboard
                });
            }
            catch (AggregateException)
            {
                return;
            }
            catch (CannotSendToUserFirstlyException)
            {
                return;
            }
        }

        public void HandleHelpMessage(BotUser user, MessageKeyboard keyboard)
        {
            var text = new MessageResponse(ResponseType.Help).response;
            SendNotification(user, text, keyboard);
        }
    }
}
