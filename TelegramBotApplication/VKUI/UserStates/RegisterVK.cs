﻿using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public class RegisterVK : CommandVK
    {
        private readonly MessageHandler messageHandler;
        private readonly IVkMessageSender vkMessageSender;
        private readonly VKUnknownMessageProcessor vkUnknownMessageProcessor;
        private readonly RegisterCommandListProvider registerCommandListProvider;
        private readonly AddingLinkCommandListProvider addingLinkCommandListProvider;
        private readonly IPeopleParser peopleParser;
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
            VKUnknownMessageProcessor vkUnknownMessageProcessor,
            IPeopleParser peopleParser,
            RegisterCommandListProvider registerCommandListProvider,
            AddingLinkCommandListProvider addingLinkCommandListProvider)
        {
            this.messageHandler = messageHandler;
            this.vkMessageSender = vkMessageSender;
            this.vkUnknownMessageProcessor = vkUnknownMessageProcessor;
            this.peopleParser = peopleParser;
            this.registerCommandListProvider = registerCommandListProvider;
            this.addingLinkCommandListProvider = addingLinkCommandListProvider;
        }

        public MessageKeyboard GetKeyboard()
        {
            return CreateKeyboard();
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            //if (messageText.Contains("http"))
            //{
            //    var splittedMessage = messageText.Split(": ");
            //    var name = splittedMessage[0];
            //    var link = splittedMessage[1];
            //    messageHandler.AddLink(user, name, link);
            //    return;
            //}

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
                case var n when n == registerCommandListProvider.GettingDiningRoom:
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
                    var newUserState = new AddingLinkVK(messageHandler, peopleParser, vkMessageSender, addingLinkCommandListProvider, vkUnknownMessageProcessor, registerCommandListProvider);
                    peopleParser.ChangeState(user.UserId, "3");
                    vkMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinksMessage).response, newUserState.GetKeyboard());
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
