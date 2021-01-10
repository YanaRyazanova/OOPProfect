using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Infrastructure;
using Telegram.Bot.Types.ReplyMarkups;
using Ninject;

namespace View.TelegramUI.UserStates
{
    public class AddingLinkTG : CommandTG
    {
        private readonly StandardKernel container;
        private readonly MessageHandler messageHandler;
        private readonly IPeopleParser peopleParser;
        private readonly ITGMessageSender tgMessageSender;
        private readonly TGUnknownMessageProcessor tgUnknownMessageProcessor;
        private readonly AddingLinkCommandListProvider addingLinkCommandListProvider;

        private static ReplyKeyboardMarkup CreateKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("Назад")
                }
            });
            return keyboard;
        }

        public ReplyKeyboardMarkup GetKeyboard()
        {
            return CreateKeyboard();
        }

        public AddingLinkTG(StandardKernel container,
            MessageHandler messageHandler,
            IPeopleParser peopleParser,
            ITGMessageSender tgMessageSender,
            TGUnknownMessageProcessor tgUnknownMessageProcessor, 
            AddingLinkCommandListProvider addingLinkCommandListProvider)
        {
            this.container = container;
            this.messageHandler = messageHandler;
            this.peopleParser = peopleParser;
            this.tgMessageSender = tgMessageSender;
            this.tgUnknownMessageProcessor = tgUnknownMessageProcessor;
            this.addingLinkCommandListProvider = addingLinkCommandListProvider;
        }

        public void ProcessMessage(string messageText, BotUser user)
        {
            if (addingLinkCommandListProvider.GetCommands().Contains(messageText))
            {
                var newUserState = container.Get<RegisterTG>();//new RegisterTG(messageHandler, tgMessageSender, tgUnknownMessageProcessor,
                    //registerCommandListProvider, peopleParser, addingLinkCommandListProvider);
                peopleParser.ChangeState(user.UserId, "2");
                tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinkCancel).response, newUserState.GetKeyboard());
            }
            
            else if (!messageText.Contains("http") && !messageText.Contains(":"))
            {
                tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.LinksError).response,
                    GetKeyboard());
            }

            else if (messageText.Contains("http") && messageText.Contains(":"))
            {
                var splittedMessage = messageText.Split(": ");
                var name = splittedMessage[0];
                var link = splittedMessage[1];
                messageHandler.AddLink(user, name, link);
                peopleParser.ChangeState(user.UserId, "2");
                var newUserState = container.Get<RegisterTG>();//new RegisterTG(messageHandler, tgMessageSender, tgUnknownMessageProcessor, registerCommandListProvider, peopleParser, addingLinkCommandListProvider);
                tgMessageSender.SendNotification(user, new MessageResponse(ResponseType.SucessfulLinks).response,
                    newUserState.GetKeyboard());
                
            }
            else
            {
                tgUnknownMessageProcessor.ProcessUnknownCommand(messageText, user, GetKeyboard(), new MessageResponse(ResponseType.Help));
            }
        }
    }
}
