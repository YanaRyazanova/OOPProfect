using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public class NotRegisterTG : CommandTG
    {
        private readonly ITGMessageSender tgItgMessageSender;
        private readonly IPeopleParser peopleParser;
        private readonly TGUnknownMessageProcessor allCommands;
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

        public NotRegisterTG(ITGMessageSender tgItgMessageSender,
            IPeopleParser peopleParser,
            TGUnknownMessageProcessor allCommands) : base(
            TgUsersStates.NotRegister)
        {
            this.tgItgMessageSender = tgItgMessageSender;
            this.peopleParser = peopleParser;
            this.allCommands = allCommands;
        }

        public override ReplyKeyboardMarkup GetKeyboard()
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
                    tgItgMessageSender.SendNotification(user, text, GetKeyboard());
                    break;
                }
                case "help":
                case "/help":
                case "помощь":
                case "помоги":
                {
                    tgItgMessageSender.HandleHelpMessage(user, GetKeyboard());
                    break;
                }
                default:
                    allCommands.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.NotRegisterError));
                    break;
            }
        }
    }
}
