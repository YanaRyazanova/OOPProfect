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
        private static MessageHandler messageHandler;
        private static IPeopleParser peopleParser;
        private static List<string> availableСommands = new List<string>{ "/start", "start", "начать", "help", "/help", "помощь", "помоги", "фт-201", "фт-202", "расписание на сегодня", "расписание на завтра", "я в столовой", "ссылки на учебные чаты"};

        public TelegramBotUI(TelegramBotClient newClient, MessageHandler newMessageHandler, IPeopleParser newPeopleParser)
        {
            client = newClient;
            messageHandler = newMessageHandler;
            peopleParser = newPeopleParser;
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
            var chatId = message.Chat.Id;
            var messageText = message.Text.ToLower();
            if (message?.Type != MessageType.Text) return;
            var currentCommand = DefineCommand(chatId.ToString());
            try
            {
                if (!IsCorrectCommand(currentCommand, messageText, chatId.ToString()))
                    return;
                switch (currentCommand.userState)
                {
                    case UsersStates.NotRegister:
                    {
                        switch (messageText)
                        {
                            case "/start":
                            case "start":
                            case "начать":
                            {
                                var text = new MessageResponse(ResponseType.Start).response;
                                currentCommand.RaiseState();
                                peopleParser.AddNewUser(chatId.ToString());
                                peopleParser.ChangeStateForUser(chatId.ToString());
                                SendNotification(chatId.ToString(), text);
                                break;
                            }
                            case "help":
                            case "/help":
                            case "помощь":
                            case "помоги":
                            {
                                HandleHelpMessage(chatId.ToString(), currentCommand.keyboard);
                                break;
                            }
                        }
                        break;
                    }
                    case UsersStates.RegisterInProcess:
                    {
                        switch (messageText)
                        {
                            case "help":
                            case "/help":
                            case "помощь":
                            case "помоги":
                            {
                                HandleHelpMessage(chatId.ToString(), currentCommand.keyboard);
                                break;
                            }
                            default:
                            {
                                if (messageHandler.GetGroup(messageText.ToUpper(), chatId.ToString()))
                                {
                                    currentCommand.RaiseState();
                                    peopleParser.ChangeStateForUser(chatId.ToString());
                                    SendNotification(chatId.ToString(), new MessageResponse(ResponseType.SucceessfulRegistration).response);
                                }
                                else
                                {
                                    SendNotification(chatId.ToString(), new MessageResponse(ResponseType.GroupError).response);
                                }
                                break;
                            }
                        }
                        break;
                    }
                    case UsersStates.Register:
                    {
                        switch (messageText)
                        {
                            case "расписание на сегодня":
                            {
                                messageHandler.GetScheduleForToday(chatId.ToString());
                                break;
                            }
                            case "расписание на завтра":
                            {
                                messageHandler.GetScheduleForNextDay(chatId.ToString());
                                break;
                            }
                            case "я в столовой":
                            {
                                var visitorsCount = messageHandler.GetDinigRoom(chatId.ToString());
                                var text = new MessageResponse(ResponseType.DiningRoom).response;
                                SendNotification(chatId.ToString(), text + visitorsCount);
                                break;
                            }
                            case "ссылки на учебные чаты":
                            {
                                messageHandler.GetLinks(chatId.ToString());
                                break;
                            }
                            case "help":
                            case "/help":
                            case "помощь":
                            case "помоги":
                            {
                                HandleHelpMessage(chatId.ToString(), currentCommand.keyboard);
                                break;
                            }
                            }
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                var text = new MessageResponse(ResponseType.CatchError).response;
                Console.WriteLine(e);
                SendNotification(chatId.ToString(), text);
            }
        }

        private bool IsCorrectCommand(CommandTG currentCommand, string messageText, string chatId)
        {
            if (availableСommands.Contains(messageText))
            {
                if (currentCommand.userState == UsersStates.NotRegister &&
                    !currentCommand.availableСommands.Contains(messageText))
                {
                    SendNotification(chatId, new MessageResponse(ResponseType.NotRegisterError).response);
                    return false;
                }
                if (currentCommand.userState == UsersStates.RegisterInProcess &&
                         !currentCommand.availableСommands.Contains(messageText))
                {
                    SendNotification(chatId, new MessageResponse(ResponseType.RegisterInProgressError).response);
                    return false;
                }

                if (currentCommand.userState == UsersStates.Register &&
                    !currentCommand.availableСommands.Contains(messageText))
                {
                    SendNotification(chatId, new MessageResponse(ResponseType.RegisterError).response);
                    return false;
                }

                return true;
            }

            SendNotification(chatId, new MessageResponse(ResponseType.Error).response);
            return false;
        }

        private void HandleHelpMessage(string chatId, ReplyKeyboardMarkup keyboard)
        {
            var text = new MessageResponse(ResponseType.Help).response;
            SendNotification(chatId, text);
        }

        private CommandTG DefineCommand(string chatID)
        {
            var userState = peopleParser.GetStateFromId(chatID);
            if (userState == "")
            {
                userState = "0";
            }
            return new CommandTG(int.Parse(userState));
        }
        public void SendNotification(string chatID, string message)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            var currentCommand = DefineCommand(chatID);
            try
            {
                client.SendTextMessageAsync(chatID, message, replyMarkup: currentCommand.keyboard).Wait();
            }
            catch (AggregateException)
            {
                return;
            }
        }

        public void SendNotificationLesson(string chatID, string message)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            var currentCommand = DefineCommand(chatID);
            client.SendTextMessageAsync(chatID, message, replyMarkup: currentCommand.keyboard).Wait();
        }
    }
}