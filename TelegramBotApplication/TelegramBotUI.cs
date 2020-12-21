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
            peopleParser.AddNewUser("23");
            peopleParser.ChangeStateForUser("23");
            peopleParser.ChangeStateForUser("1");
            peopleParser.ChangeGroup("23", "52");
            peopleParser.ChangeGroup("1", "54");
            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;
            var messageText = message.Text.ToLower();
            if (message?.Type != MessageType.Text) return;
            
            var userState = peopleParser.GetStateFromId(chatId.ToString());
            if (userState == "")
            {
                userState = "0";
            }
            var currentCommand = new Command(int.Parse(userState));
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
                                SendSubsidiaryNotification(chatId, text, currentCommand.keyboard);
                                currentCommand.RaiseState();
                                peopleParser.AddNewUser(chatId.ToString());
                                peopleParser.ChangeStateForUser(chatId.ToString());
                                Console.WriteLine(peopleParser.GetStateFromId(chatId.ToString()));
                                break;
                            }
                            case "help":
                            case "/help":
                            case "помощь":
                            case "помоги":
                            {
                                HandleHelpMessage(chatId, currentCommand.keyboard);
                                break;
                            }
                        }
                        break;
                    }

                    case UsersStates.RegisterInProcess:
                    {
                        messageHandler.GetGroup(messageText.ToUpper(), chatId);
                        SendSubsidiaryNotification(chatId, "Выберите пункт меню", currentCommand.keyboard);
                        currentCommand.RaiseState();
                        peopleParser.ChangeStateForUser(chatId.ToString());
                        Console.WriteLine(peopleParser.GetStateFromId(chatId.ToString()));
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
                        }
                        break;
                    }
                }
                //switch (messageText)
                //{
                //    case "/start":
                //    {
                //        var text = new MessageResponse(ResponseType.Start).response;
                //        SendSubsidiaryNotification(chatId, text, keyboardStart);
                //        break;
                //    }
                //    case "help":
                //    case "/help":
                //    case "помощь":
                //    case "помоги":
                //    {
                //        HandleHelpMessage(chatId);
                //        break;
                //    }
                //    case "расписание на сегодня":
                //    {
                //        messageHandler.GetScheduleForToday(chatId.ToString());
                //        break;
                //    }
                //    case "расписание на завтра":
                //    {
                //        messageHandler.GetScheduleForNextDay(chatId.ToString());
                //        break;
                //    }
                //    case "я в столовой":
                //    {
                //        messageHandler.GetDinigRoom(chatId.ToString());
                //        break;
                //    }
                //    case "фт-201":
                //    case "фт-202":
                //    {
                //        messageHandler.GetGroup(messageText.ToUpper(), chatId);
                //        SendSubsidiaryNotification(chatId, "Выберите пункт меню", keyboardMenu);
                //        break;
                //    }
                //    default:
                //    {
                //        var text = new MessageResponse(ResponseType.Error).response;
                //        SendNotification(chatId.ToString(), text);
                //        break;
                //    }
                //}
            }
            catch (Exception e)
            {
                if (e.Message == "constraint failed\r\nUNIQUE constraint failed: PeopleAndGroups.ChatID")
                {
                    var text = "Вы уже зарегистрированы в боте. Смену группы мы добавим позже :-)";
                    await client.SendTextMessageAsync(chatId, text, replyMarkup: CreateKeyboard());
                }
                var textt = "Упс! Кажется что-то пошло не так!Попробуйте начать с команды '/start'";
                Console.WriteLine(e);
                SendSubsidiaryNotification(chatId, textt, currentCommand.keyboard);
            }
        }

        private void CorrectnessCheck(Command currentCommand, string messageText, long chatId)
        {
            if (!currentCommand.availableСommands.Contains(messageText))
                HandleErrorMessage(chatId, currentCommand.keyboard);
        }

        private void HandleErrorMessage(long chatId, ReplyKeyboardMarkup keyboard)
        {
            SendSubsidiaryNotification(chatId, 
                "К сожалению, бот не умеет обрабатывать такую команду :-( Отправьте сообщение 'help' или 'помощь', чтобы увидеть все команды бота.",
                keyboard);
        }
        private void HandleHelpMessage(long chatId, ReplyKeyboardMarkup keyboard)
        {
            var text = new MessageResponse(ResponseType.Help).response;
            SendSubsidiaryNotification(chatId, text, keyboard);
        }

        public void SendNotification(string chatID, string message)
        {
            if (message is null)
                message = "У вас сегодня нет пар, отдыхайте!";
            client.SendTextMessageAsync(chatID, message, replyMarkup: keyboardMenu).Wait();
        }

        private void SendSubsidiaryNotification(long chatID, string text, ReplyKeyboardMarkup keyboard)
        {
            client.SendTextMessageAsync(chatID, text, replyMarkup: keyboard).Wait();
        }

        public void SendNotificationLesson(string chatID, string message)
        {
            client.SendTextMessageAsync(chatID, message, replyMarkup: keyboardMenu).Wait();
        }
    }
}