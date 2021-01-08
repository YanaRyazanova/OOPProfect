using System;
using System.Collections.Generic;
using Application;
using Infrastructure;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public class NotRegisterVK : CommandVK
    {
        private readonly IVkMessageSender vkMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly MessageHandler messageHandler;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;
        private readonly GroupProvider groupProvider;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private readonly AddingLinkCommandListProvider addingLinkCommandListProvider;


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

        public NotRegisterVK(
            IVkMessageSender vkMessageSender,
            IPeopleParser peopleParser,
            MessageHandler messageHandler,
            VKUnknownMessageProcessor vkUnknownMessageProcessor,
            GroupProvider groupProvider,
            RegisterCommandListProvider registerCommandListProvider,
            AddingLinkCommandListProvider addingLinkCommandListProvider
            )
        {
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.messageHandler = messageHandler;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
            this.groupProvider = groupProvider;
            this.registerCommandListProvider = registerCommandListProvider;
            this.addingLinkCommandListProvider = addingLinkCommandListProvider;
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
                        peopleParser.AddNewUser(user.UserId);
                        peopleParser.EvaluateState(user.UserId);
                        var updatedState = new RegisterInProcessVK(messageHandler, vkMessageSender, peopleParser, vkUnknownMessageProcessor, groupProvider, registerCommandListProvider, addingLinkCommandListProvider);
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
