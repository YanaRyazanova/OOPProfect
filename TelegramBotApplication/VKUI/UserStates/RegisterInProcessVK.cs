using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using View.VKUI;
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
        private static MessageKeyboard CreateStartKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "ФТ-201",
                        Type = KeyboardButtonActionType.Text
                    },

                    Color = KeyboardButtonColor.Primary
                },
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "ФТ-202",
                        Type = KeyboardButtonActionType.Text
                    },

                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        public RegisterInProcessVK(MessageHandler messageHandler, VKMessageSender vkMessageSender, IPeopleParser peopleParser, VKUnknownMessageProcessor vkUnknownMessageProcessor) : base(
            VkUsersStates.RegisterInProcess)
        {
            this.messageHandler = messageHandler;
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
        }

        public override MessageKeyboard GetKeyboard()
        {
            return CreateStartKeyboard();
        }

        public override void ProcessMessage(string messageText, VKUser userId)
        {
            switch (messageText)
            {
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                {
                    vkMessageSender.HandleHelpMessage(userId, GetKeyboard());
                    break;
                }
                default:
                {
                    if (messageHandler.GetAllGroups().Contains(messageText))
                    {
                        if (messageHandler.GetGroup(messageText.ToUpper(), userId.ToString()))
                        {
                            RaiseState();
                            peopleParser.ChangeStateForUser(userId.userID.ToString());
                            vkMessageSender.SendNotification(userId, new MessageResponse(ResponseType.SucceessfulRegistration).response, GetKeyboard());
                        }
                        else
                        {
                            vkMessageSender.SendNotification(userId, new MessageResponse(ResponseType.GroupError).response, GetKeyboard());
                        }
                    }
                    else
                    {
                        vkUnknownMessageProcessor.ProcessUnknownCommand(messageText, userId, GetKeyboard(), new MessageResponse(ResponseType.RegisterInProgressError));
                    }

                    break;
                }
            }
        }
    }
}
