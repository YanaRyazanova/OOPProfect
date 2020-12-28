using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public class NotRegister : CommandTG
    {
        private readonly IMessageSender tgMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly UnknownMessageProcessor allCommands;
        private static ReplyKeyboardMarkup CreatePreStartKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("Начать")
                }
            });
            return keyboard;
        }

        public NotRegister(IMessageSender tgMessageSender, IPeopleParser peopleParser, UnknownMessageProcessor allCommands) : base(
            UsersStates.NotRegister)
        {
            this.tgMessageSender = tgMessageSender;
            this.peopleParser = peopleParser;
            this.allCommands = allCommands;
        }

        public override ReplyKeyboardMarkup GetKeyboard()
        {
            return CreatePreStartKeyboard();
        }

        public override void ProcessMessage(string messageText, TGUser chatId)
        {
            switch (messageText)
            {
                case "/start":
                case "start":
                case "начать":
                {
                    var text = new MessageResponse(ResponseType.Start).response; 
                    RaiseState();
                    peopleParser.AddNewUser(chatId.ToString());
                    peopleParser.ChangeStateForUser(chatId.ToString());
                    tgMessageSender.SendNotification(chatId, text, GetKeyboard());
                    break;
                }
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                {
                    tgMessageSender.HandleHelpMessage(chatId, GetKeyboard());
                    break;
                }
                default:
                    allCommands.ProcessUnknownCommand(messageText, chatId, GetKeyboard(), new MessageResponse(ResponseType.NotRegisterError));
                    break;
            }
        }
    }
}
