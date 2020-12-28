using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Application;
using Domain;
using Infrastructure;
using Infrastructure.Csv;
using Infrastructure.SQL;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

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
            var chatId = new TGUser(message.Chat.Id);
            var messageText = message.Text.ToLower();
            if (message?.Type != MessageType.Text) return;
            var currentCommand = DefineCommand(chatId.ToString());
            try
            {
                currentCommand.ProcessMessage(messageText, chatId);
            }
            catch (Exception e)
            {
                var text = new MessageResponse(ResponseType.CatchError).response;
                Console.WriteLine(e);
                tgMessageSender.SendNotification(chatId, text, currentCommand.GetKeyboard());
            }
        }

        private CommandTG DefineCommand(string chatID)
        {
            var userState = peopleParser.GetStateFromId(chatID);
            if (userState == "")
            {
                userState = "0";
            }
            return commandTgFactory.Create(int.Parse(userState));
        }
        
        public void SendNotificationLesson(string chatID, string message)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            try
            {
                var currentCommand = DefineCommand(chatID);
                client.SendTextMessageAsync(chatID, message, replyMarkup: currentCommand.GetKeyboard()).Wait();
            }
            catch (AggregateException)
            {
                return;
            }
        }
    }
}