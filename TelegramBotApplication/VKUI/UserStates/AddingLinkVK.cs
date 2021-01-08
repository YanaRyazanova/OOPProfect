using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public class AddingLinkVK : CommandVK
    {
        private readonly MessageHandler messageHandler;
        private readonly IPeopleParser peopleParser;
        private readonly IVkMessageSender vkMessageSender;
        private readonly AddingLinkCommandListProvider addingLinkCommandListProvider;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;
        private readonly RegisterCommandListProvider registerCommandListProvider;

        public AddingLinkVK(MessageHandler messageHandler,
            IPeopleParser peopleParser,
            IVkMessageSender vkMessageSender,
            AddingLinkCommandListProvider addingLinkCommandListProvider,
            VKUnknownMessageProcessor vkUnknownMessageProcessor,
            RegisterCommandListProvider registerCommandListProvider)
        {
            this.messageHandler = messageHandler;
            this.peopleParser = peopleParser;
            this.vkMessageSender = vkMessageSender;
            this.addingLinkCommandListProvider = addingLinkCommandListProvider;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
            this.registerCommandListProvider = registerCommandListProvider;
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
            if (addingLinkCommandListProvider.GetCommands().Contains(messageText))
            {
                var newUserState = new RegisterVK(messageHandler, vkMessageSender, vkUnknownMessageProcessor,
                    peopleParser, registerCommandListProvider, addingLinkCommandListProvider);
                peopleParser.ChangeState(user.UserId, "2");
                vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinkCancel).response, newUserState.GetKeyboard());
            }

            else if (!messageText.Contains("http") && !messageText.Contains(":"))
            {
                vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinksError).response,
                    GetKeyboard());
            }

            else if (messageText.Contains("http") && messageText.Contains(":"))
            {
                var splittedMessage = messageText.Split(": ");
                var name = splittedMessage[0];
                var link = splittedMessage[1];
                messageHandler.AddLink(user, name, link);
                var newUserState = new RegisterVK(messageHandler, vkMessageSender, vkUnknownMessageProcessor,
                    peopleParser, registerCommandListProvider, addingLinkCommandListProvider);
                peopleParser.ChangeState(user.UserId, "2");
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
