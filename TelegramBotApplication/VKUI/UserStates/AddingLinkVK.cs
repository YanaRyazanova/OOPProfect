using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using Ninject;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public class AddingLinkVK : CommandVK
    {
        private readonly StandardKernel container;
        private readonly MessageHandler messageHandler;
        private readonly IPeopleParser peopleParser;
        private readonly VKMessageSender vkMessageSender;
        private readonly AddingLinkCommandListProvider addingLinkCommandListProvider;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;

        public AddingLinkVK(
            StandardKernel container,
            MessageHandler messageHandler,
            IPeopleParser peopleParser,
            VKMessageSender vkMessageSender,
            AddingLinkCommandListProvider addingLinkCommandListProvider,
            VKUnknownMessageProcessor vkUnknownMessageProcessor)
        {
            this.container = container;
            this.messageHandler = messageHandler;
            this.peopleParser = peopleParser;
            this.vkMessageSender = vkMessageSender;
            this.addingLinkCommandListProvider = addingLinkCommandListProvider;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
        }

        private static MessageKeyboard CreateKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Назад", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        public MessageKeyboard GetKeyboard()
        {
            return CreateKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            var httpConstant = "http";
            var colonConstant = ":";
            if (addingLinkCommandListProvider.GetCommands().Contains(messageText))
            {
                var newUserState = container.Get<RegisterVK>();
                peopleParser.ChangeState(user.UserId, UserStates.Register);
                vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinkCancel).response, newUserState.GetKeyboard());
            }

            else if (!messageText.Contains(httpConstant) && !messageText.Contains(colonConstant))
            {
                vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinksError).response,
                    GetKeyboard());
            }

            else if (messageText.Contains(httpConstant) && messageText.Contains(colonConstant))
            {
                var splittedMessage = messageText.Split(": ");
                var name = splittedMessage[0];
                var link = splittedMessage[1];
                messageHandler.AddLink(user, name, link);
                var newUserState = container.Get<RegisterVK>();
                peopleParser.ChangeState(user.UserId, UserStates.Register);
                vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.SucessfulLinks).response,
                    newUserState.GetKeyboard());
            }

            else
            {
                vkUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.Help));
            }
        }
    }
}
