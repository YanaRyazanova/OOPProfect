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
        private readonly VKUnknownMessageProcessor allCommands;
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

        public NotRegisterVK(IVkMessageSender vkMessageSender, IPeopleParser peopleParser, VKUnknownMessageProcessor allCommands) : base(
            VkUsersStates.NotRegister)
        {
            this.vkMessageSender = vkMessageSender;
            this.peopleParser = peopleParser;
            this.allCommands = allCommands;
        }

        public override MessageKeyboard GetKeyboard()
        {
            return CreatePreStartKeyboard();
        }

        public override void ProcessMessage(string messageText, BotUser user)
        {
            switch (messageText)
            {
                case "/start":
                case "start":
                case "начать":
                    {
                        var text = new MessageResponse(ResponseType.Start).response;
                        RaiseState();
                        peopleParser.AddNewUser(user.UserId);
                        peopleParser.ChangeStateForUser(user.UserId);
                        vkMessageSender.SendNotification(user, text, GetKeyboard());
                        break;
                    }
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                    {
                        vkMessageSender.HandleHelpMessage(user, GetKeyboard());
                        break;
                    }
                default:
                    allCommands.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.NotRegisterError));
                    break;
            }
        }
    }
}
