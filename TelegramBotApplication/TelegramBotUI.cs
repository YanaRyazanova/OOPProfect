using System;
using System.Collections.Generic;
using System.Linq;
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


        private ReplyKeyboardMarkup keyboardMenu;

        public TelegramBotUI(TelegramBotClient newClient, MessageHandler newMessageHandler, IPeopleParser newPeopleParser)
        {
            client = newClient;
            messageHandler = newMessageHandler;
            peopleParser = newPeopleParser;
        }
        private static ReplyKeyboardMarkup CreateKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("Расписание на сегодня"),
                    new KeyboardButton("Расписание на завтра")
                },

                new[]
                {
                    new KeyboardButton("Я в столовой"),
                    new KeyboardButton("Help")
                }
            });
            return keyboard;
        }

        public void Run()
        {
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;
            var messageText = message.Text.ToLower();
            if (message?.Type != MessageType.Text) return;
            var currentCommand = DefineCommand(chatId.ToString());
            try
            {
                CorrectnessCheck(currentCommand, messageText, chatId);
                switch (currentCommand.userState)
                {
                    case UsersStates.NotRegister:
                    {
                        switch (messageText)
                        {
                            case "/start":
                            case "start":
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
                                messageHandler.GetGroup(messageText.ToUpper(), chatId);
                                currentCommand.RaiseState();
                                peopleParser.ChangeStateForUser(chatId.ToString());
                                SendNotification(chatId.ToString(), "Вы успешно зарегистрированы! Выберите пункт меню");
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
                                messageHandler.GetDinigRoom(chatId.ToString());
                                break;
                            }
                            case "ссылки":
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
                var text = "Упс! Кажется что-то пошло не так!Попробуйте начать с команды '/start'";
                Console.WriteLine(e);
                SendNotification(chatId.ToString(), text);
            }
        }

        private void CorrectnessCheck(Command currentCommand, string messageText, long chatId)
        {
            if (!currentCommand.availableСommands.Contains(messageText))
                HandleErrorMessage(chatId.ToString());
        }

        private void HandleErrorMessage(string chatId)
        {
            SendNotification(chatId, 
                "К сожалению, бот не умеет обрабатывать такую команду :-( Отправьте сообщение 'help' или 'помощь', чтобы увидеть все команды бота.");
        }
        private void HandleHelpMessage(string chatId, ReplyKeyboardMarkup keyboard)
        {
            var text = new MessageResponse(ResponseType.Help).response;
            SendNotification(chatId, text);
        }

        private Command DefineCommand(string chatID)
        {
            var userState = peopleParser.GetStateFromId(chatID);
            if (userState == "")
            {
                userState = "0";
            }
            return new Command(int.Parse(userState));
        }
        public void SendNotification(string chatID, string message)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            var currentCommand = DefineCommand(chatID);
            client.SendTextMessageAsync(chatID, message, replyMarkup: currentCommand.keyboard).Wait();
        }

        public void SendNotificationLesson(string chatID, string message)
        {
            client.SendTextMessageAsync(chatID, message, replyMarkup: keyboardMenu).Wait();
        }
    }
}