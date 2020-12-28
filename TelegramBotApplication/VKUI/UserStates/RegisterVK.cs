using System;
using System.Collections.Generic;
using System.Text;
using Application;
using View.VKUI;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public class RegisterVK : CommandVK
    {
        private readonly MessageHandler messageHandler;
        private readonly VKMessageSender vkMessageSender;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;
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
                        Label = "Расписание на сегодня", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                },
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Расписание на завтра", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            var line2 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Я в столовой", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                },
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Ссылки на учебные чаты", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                },
            };
            var line3 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Help", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            buttonsList.Add(line2);
            buttonsList.Add(line3);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        public RegisterVK(MessageHandler messageHandler, VKMessageSender vkMessageSender, VKUnknownMessageProcessor vkUnknownMessageProcessor) : base(VkUsersStates.Register)
        {
            this.messageHandler = messageHandler;
            this.vkMessageSender = vkMessageSender;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
        }

        public override MessageKeyboard GetKeyboard()
        {
            return CreateKeyboard();
        }

        public override void ProcessMessage(string messageText, VKUser userId)
        {
            switch (messageText)
            {
                case "расписание на сегодня":
                {
                    messageHandler.GetScheduleForToday(userId.ToString());
                    break;
                }
                case "расписание на завтра":
                {
                    messageHandler.GetScheduleForNextDay(userId.ToString());
                    break;
                }
                case "я в столовой":
                {
                    var visitorsCount = messageHandler.GetDinigRoom(userId.ToString());
                    var text = new MessageResponse(ResponseType.DiningRoom).response;
                    vkMessageSender.SendNotification(userId, text + visitorsCount, GetKeyboard());
                    break;
                }
                case "ссылки на учебные чаты":
                {
                    messageHandler.GetLinks(userId.userID.ToString());
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
                {
                    vkUnknownMessageProcessor.ProcessUnknownCommand(messageText, userId, GetKeyboard(), new MessageResponse(ResponseType.RegisterError));
                    break;
                }
            }
        }
    }
}
