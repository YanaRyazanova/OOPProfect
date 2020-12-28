using System;
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
        private readonly TGMessageSender tgMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        private ReplyKeyboardMarkup CreateStartKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                messageHandler
                    .GetAllGroups()
                    .Select(x => new KeyboardButton(x))
                    .ToArray()
            });
            return keyboard;
        }

        public RegisterInProcessTG(MessageHandler messageHandler, TGMessageSender tgMessageSender, IPeopleParser peopleParser, TGUnknownMessageProcessor tgUnknownMessageProcessor) : base(
            TgUsersStates.RegisterInProcess)
        {
            this.messageHandler = messageHandler;
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
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
                    if (messageHandler.GetAllGroups().Contains(messageText))
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