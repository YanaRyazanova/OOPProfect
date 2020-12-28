using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;
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

        public override void ProcessMessage(string messageText, VKUser userId)
        {
            switch (messageText)
            {
                case "/start":
                case "start":
                case "начать":
                    {
                        var text = new MessageResponse(ResponseType.Start).response;
                        RaiseState();
                        peopleParser.AddNewUser(userId.ToString());
                        peopleParser.ChangeStateForUser(userId.ToString());
                        vkMessageSender.SendNotification(userId, text, GetKeyboard());
                        break;
                    }
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                    {
                        vkMessageSender.HandleHelpMessage(userId, GetKeyboard());
                        break;
                    }
                default:
                    allCommands.ProcessUnknownCommand(messageText, userId, GetKeyboard(), new MessageResponse(ResponseType.NotRegisterError));
                    break;
            }
        }
    }
}
