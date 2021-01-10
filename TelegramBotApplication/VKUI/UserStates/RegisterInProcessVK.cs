using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application;
using Infrastructure;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;
using Ninject;

namespace View
{
    public class RegisterInProcessVK : CommandVK
    {
        private readonly StandardKernel container;
        private readonly MessageHandler messageHandler;
        private readonly VKMessageSender vkMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;
        private readonly GroupProvider groupProvider;

        private MessageKeyboard CreateStartKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var buttons = groupProvider
                .GetAllGroups()
                .Select(x => new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = x,
                        Type = KeyboardButtonActionType.Text
                    },

                    Color = KeyboardButtonColor.Primary
                })
                .ToList();
            buttonsList.Add(buttons);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        public RegisterInProcessVK(StandardKernel container,
            MessageHandler messageHandler,
            VKMessageSender vkMessageSender,
            IPeopleParser peopleParser,
            VKUnknownMessageProcessor vkUnknownMessageProcessor,
            GroupProvider groupProvider)
        {
            this.container = container;
            this.messageHandler = messageHandler;
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
            this.groupProvider = groupProvider;
        }

        public MessageKeyboard GetKeyboard()
        {
            return CreateStartKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            if (groupProvider.GetAllGroups().Contains(messageText.ToUpper()))
            {
                if (messageHandler.SaveGroup(messageText.ToUpper(), user))
                {
                    peopleParser.EvaluateState(user.UserId);
                    var updatedState = container.Get<RegisterVK>();
                    vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.SuccessfulRegistration).response, updatedState.GetKeyboard());
                }
                else
                {
                    vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.GroupError).response, GetKeyboard());
                }
            }
            else
            {
                vkUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.RegisterInProgressError));
            }
        }
    }
}
