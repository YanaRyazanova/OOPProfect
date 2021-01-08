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

        public AddingLinkVK(MessageHandler messageHandler,
            IPeopleParser peopleParser,
            IVkMessageSender vkMessageSender)
        {
            this.messageHandler = messageHandler;
            this.peopleParser = peopleParser;
            this.vkMessageSender = vkMessageSender;
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

        public MessageKeyboard GetKeyboard()
        {
            return CreateKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            if (!messageText.Contains("http"))
                vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinksError).response, GetKeyboard());
            var splittedMessage = messageText.Split(": ");
            var name = splittedMessage[0];
            var link = splittedMessage[1];
            messageHandler.AddLink(user, name, link);
            vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.SucessfulLinks).response, GetKeyboard());
            peopleParser.ChangeState(user.UserId, "2");
        }
    }
}
