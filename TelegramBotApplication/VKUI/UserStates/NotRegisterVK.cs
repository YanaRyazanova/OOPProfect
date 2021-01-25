using System;
using System.Collections.Generic;
using Application;
using Infrastructure;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;
using  Ninject;

namespace View
{
    public class NotRegisterVK : CommandVK
    {
        private readonly StandardKernel container;
        private readonly VKMessageSender vkMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;


        private static MessageKeyboard CreatePreStartKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Начать", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        public NotRegisterVK(StandardKernel container,
            VKMessageSender vkMessageSender,
            IPeopleParser peopleParser,
            VKUnknownMessageProcessor vkUnknownMessageProcessor)
        {
            this.container = container;
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
        }

        public MessageKeyboard GetKeyboard()
        {
            return CreatePreStartKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            switch (messageText)
            {
                case "/start":
                case "start":
                case "начать":
                    {
                        var text = new MessageResponse(ResponseType.Start).response;
                        peopleParser.AddNewUser(user.UserId, "0","vk");
                        peopleParser.EvaluateState(user.UserId);
                        var updatedState = container.Get<RegisterInProcessVK>();
                        vkMessageSender.SendNotification(user, text, updatedState.GetKeyboard());
                        break;
                    }
                default:
                    vkUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.NotRegisterError));
                    break;
            }
        }
    }
}
