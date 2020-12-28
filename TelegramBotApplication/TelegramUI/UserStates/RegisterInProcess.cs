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
    public class RegisterInProcess : CommandTG
    {
        private readonly MessageHandler messageHandler;
        private readonly TGMessageSender tgMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly UnknownMessageProcessor unknownMessageProcessor;
        private  ReplyKeyboardMarkup CreateStartKeyboard()
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

        public RegisterInProcess(MessageHandler messageHandler, TGMessageSender tgMessageSender, IPeopleParser peopleParser, UnknownMessageProcessor unknownMessageProcessor) : base(
            UsersStates.RegisterInProcess)
        {
            this.messageHandler = messageHandler;
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.unknownMessageProcessor = unknownMessageProcessor;
        }

        public override ReplyKeyboardMarkup GetKeyboard()
        {
            return CreateStartKeyboard();
        }

        public override void ProcessMessage(string messageText, TGUser chatId)
        {
            switch (messageText)
            {
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                {
                    tgMessageSender.HandleHelpMessage(chatId, GetKeyboard());
                    break;
                }
                default:
                {
                    if (messageHandler.GetAllGroups().Contains(messageText.ToUpper()))
                    {
                        if (messageHandler.GetGroup(messageText.ToUpper(), chatId.userID.ToString()))
                        {
                            RaiseState();
                            peopleParser.ChangeStateForUser(chatId.userID.ToString());
                            tgMessageSender.SendNotification(chatId, new MessageResponse(ResponseType.SucceessfulRegistration).response, GetKeyboard());
                        }
                        else
                        {
                            tgMessageSender.SendNotification(chatId, new MessageResponse(ResponseType.GroupError).response, GetKeyboard());
                        }
                    }
                    else
                    {
                        unknownMessageProcessor.ProcessUnknownCommand(messageText, chatId, GetKeyboard(), new MessageResponse(ResponseType.RegisterInProgressError));
                    }
                    
                    break;
                }
            }
        }
    }
}