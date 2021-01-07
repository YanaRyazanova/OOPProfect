using System;
using System.Collections.Generic;
using System.Text;
using Application;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public class RegisterVK : CommandVK
    {
        private readonly MessageHandler messageHandler;
        private readonly IVkMessageSender vkMessageSender;
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
                        Label = "Добавить ссылку на чат", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                },
                
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

        public RegisterVK(
            MessageHandler messageHandler,
            IVkMessageSender vkMessageSender,
            VKUnknownMessageProcessor vkUnknownMessageProcessor) : base(VkUsersStates.Register)
        {
            this.messageHandler = messageHandler;
            this.vkMessageSender = vkMessageSender;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
        }

        public override MessageKeyboard GetKeyboard()
        {
            return CreateKeyboard();
        }

        public override void ProcessMessage(string messageText, BotUser user)
        {
            if (messageText.Contains("https"))
            {
                var splittedMessage = messageText.Split(": ");
                var name = splittedMessage[0];
                var link = splittedMessage[1];
                messageHandler.AddLink(user, name, link);
                return;
            }

            switch (messageText)
            {
                case "расписание на сегодня":
                {
                    messageHandler.GetScheduleForToday(user);
                    break;
                }
                case "расписание на завтра":
                {
                    messageHandler.GetScheduleForNextDay(user);
                    break;
                }
                case "я в столовой":
                {
                    var visitorsCount = messageHandler.GetDiningRoom(user);
                    var text = new MessageResponse(ResponseType.DiningRoom).response;
                    vkMessageSender.SendNotification(user, text + visitorsCount, GetKeyboard());
                    break;
                }
                case "ссылки на учебные чаты":
                {
                    messageHandler.GetLinks(user);
                    break;
                }
                case "добавить ссылку на чат":
                {
                    //messageHandler.AskForLink(user);
                    break;
                }
                default:
                {
                    vkUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.RegisterError));
                    break;
                }
            }
        }
    }
}
