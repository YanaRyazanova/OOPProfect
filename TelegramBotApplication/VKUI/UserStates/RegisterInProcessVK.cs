using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application;
using Infrastructure;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public class RegisterInProcessVK : CommandVK
    {
        private readonly MessageHandler messageHandler;
        private readonly IVkMessageSender vkMessageSender;
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

        public RegisterInProcessVK(MessageHandler messageHandler,
            IVkMessageSender vkMessageSender,
            IPeopleParser peopleParser,
            VKUnknownMessageProcessor vkUnknownMessageProcessor,
            GroupProvider groupProvider) : base(
            VkUsersStates.RegisterInProcess)
        {
            this.messageHandler = messageHandler;
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
            this.groupProvider = groupProvider;
        }

        public override MessageKeyboard GetKeyboard()
        {
            return CreateStartKeyboard();
        }

        public override void ProcessMessage(string messageText, BotUser user)
        {
            if (groupProvider.GetAllGroups().Contains(messageText.ToUpper()))
            {
                if (messageHandler.SaveGroup(messageText.ToUpper(), user))
                {
                    peopleParser.EvaluateState(user.UserId);
                    var updatedState = new RegisterVK(messageHandler, vkMessageSender, vkUnknownMessageProcessor);
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
