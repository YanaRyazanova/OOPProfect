using System;
using Application;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Infrastructure;

namespace View
{
    public class TelegramBotUI
    {
        private static TelegramBotClient client;
        private static IPeopleParser peopleParser;
        private readonly CommandTGFactory commandTgFactory;
        private readonly TGMessageSender tgMessageSender;

        public TelegramBotUI(
            TelegramBotClient newClient,
            IPeopleParser newPeopleParser,
            CommandTGFactory commandTgFactory,
            TGMessageSender tgMessageSender)
        {
            client = newClient;
            peopleParser = newPeopleParser;
            this.commandTgFactory = commandTgFactory;
            this.tgMessageSender = tgMessageSender;
        }

        public void Run()
        {
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            Console.WriteLine(message);
            var user = new BotUser(message.Chat.Id.ToString());
            var messageText = message.Text.ToLower();
            if (message?.Type != MessageType.Text) return;
            var currentCommand = DefineCommand(user);
            try
            {
                currentCommand.ProcessMessage(messageText, user);
            }
            catch (Exception e)
            {
                var text = new MessageResponse(ResponseType.CatchError).response;
                Console.WriteLine(e);
                tgMessageSender.SendNotification(user, text, currentCommand.GetKeyboard());
            }
        }

        private CommandTG DefineCommand(BotUser user)
        {
            var userState = peopleParser.GetStateFromId(user.UserId);
            if (userState == "")
            {
                userState = "0";
            }
            return commandTgFactory.Create(int.Parse(userState));
        }
        
        public void SendMessage(BotUser user, string message)
        {
            var currentCommand = DefineCommand(user);
            tgMessageSender.SendNotification(user, message, currentCommand.GetKeyboard());
            //if (message is null)
            //    message = "У вас сегодня нет пар, отдыхайте!";
            //try
            //{
            //    
            //    client.SendTextMessageAsync(user.UserId, message, replyMarkup: currentCommand.GetKeyboard()).Wait();
            //}
            //catch (AggregateException)
            //{
            //    return;
            //}
        }
    }
}