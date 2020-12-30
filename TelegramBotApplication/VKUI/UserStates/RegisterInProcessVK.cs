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
            //var buttonsList = new List<List<MessageKeyboardButton>>();
            //var line1 = new List<MessageKeyboardButton>
            //{
            //    new MessageKeyboardButton
            //    {
            //        Action = new MessageKeyboardButtonAction
            //        {
            //            Label = "ФТ-201",
            //            Type = KeyboardButtonActionType.Text
            //        },

            //        Color = KeyboardButtonColor.Primary
            //    },
            //    new MessageKeyboardButton
            //    {
            //        Action = new MessageKeyboardButtonAction
            //        {
            //            Label = "ФТ-202",
            //            Type = KeyboardButtonActionType.Text
            //        },

            //        Color = KeyboardButtonColor.Primary
            //    }
            //};
            //buttonsList.Add(line1);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        public RegisterInProcessVK(MessageHandler messageHandler,
            VKMessageSender vkMessageSender,
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
            switch (messageText)
            {
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                {
                    vkMessageSender.HandleHelpMessage(user, GetKeyboard());
                    break;
                }
                default:
                {
                    if (groupProvider.GetAllGroups().Contains(messageText.ToUpper()))
                    {
                        if (messageHandler.GetGroup(messageText.ToUpper(), user))
                        {
                            RaiseState();
                            peopleParser.ChangeStateForUser(user.UserId);
                            vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.SucceessfulRegistration).response, GetKeyboard());
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

                    break;
                }
            }
        }
    }
}
