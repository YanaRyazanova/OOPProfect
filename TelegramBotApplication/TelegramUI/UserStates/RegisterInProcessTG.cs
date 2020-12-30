﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public class RegisterInProcessTG : CommandTG
    {
        private readonly MessageHandler messageHandler;
        private readonly ITGMessageSender tgMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        private readonly GroupProvider groupProvider;

        private ReplyKeyboardMarkup CreateStartKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                groupProvider
                    .GetAllGroups()
                    .Select(x => new KeyboardButton(x))
                    .ToArray()
            });
            return keyboard;
        }

        public RegisterInProcessTG(MessageHandler messageHandler,
            ITGMessageSender tgMessageSender,
            IPeopleParser peopleParser,
            TGUnknownMessageProcessor tgUnknownMessageProcessor,
            GroupProvider groupProvider) : base(
            TgUsersStates.RegisterInProcess)
        {
            this.messageHandler = messageHandler;
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.groupProvider = groupProvider;
        }

        public override ReplyKeyboardMarkup GetKeyboard()
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
                    tgMessageSender.HandleHelpMessage(user, GetKeyboard());
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
                            tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.SucceessfulRegistration).response, GetKeyboard());
                        }
                        else
                        {
                            tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.GroupError).response, GetKeyboard());
                        }
                    }
                    else
                    {
                        tgUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.RegisterInProgressError));
                    }
                    
                    break;
                }
            }
        }
    }
}