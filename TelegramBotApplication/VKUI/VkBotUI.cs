using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Domain;
using Infrastructure;
using Infrastructure.Csv;
using Infrastructure.SQL;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace View
{
    public class VkBotUI
    {
        private static VkApi vkApi;
        private static string vkToken;
        private static long userID;
        private static ulong? ts;
        private static ulong? pts;
        private static Timer watchTimer = null;
        private static byte maxSleepSteps = 3;
        private static int stepSleepTime = 333;
        private static byte currentSleepSteps = 1;
        private delegate void MessagesRecievedDelegate(VkApi owner, ReadOnlyCollection<Message> messages);
        private static event MessagesRecievedDelegate NewMessages;
        private static MessageHandler messageHandler;
        private Dictionary<string, DateTime> usersLastNotify = new Dictionary<string, DateTime>();
        private static Random rnd = new Random();
        private static List<string> availableСommands = new List<string> { "/start", "start", "начать", "help", "/help", "помощь", "помоги", "фт-201", "фт-202", "расписание на сегодня", "расписание на завтра", "я в столовой", "ссылки на учебные чаты" };
        private static IPeopleParser peopleParser;
        public VkBotUI(VkApi api, string keyVkToken, MessageHandler handler, IPeopleParser newPeopleParser)
        {
            vkApi = api;
            vkToken = keyVkToken;
            messageHandler = handler;
            peopleParser = newPeopleParser;
        }

        public void Run()
        {
            Console.WriteLine("Попытка авторизации...");
            if (Auth(vkToken))
            {
                Console.WriteLine("VkBot is successfully authorized");
                Eye();
            }
            else
            {
                Console.WriteLine("Could not authorize VkBot");
            }

            Console.WriteLine("Нажмите ENTER чтобы выйти...");
        }

        private static bool Auth(string token)
        {
            try
            {
                vkApi.Authorize(new ApiAuthParams { AccessToken = token });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private void Answer(string message)
        {
            var messageText = message.ToLower();
            var currentCommand = DefineCommand(userID.ToString());
            try
            {
                if (!IsCorrectCommand(currentCommand, messageText))
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
                                        peopleParser.AddNewUser(userID.ToString());
                                        peopleParser.ChangeStateForUser(userID.ToString());
                                        SendMessage(userID, text);
                                        break;
                                    }
                                case "help":
                                case "/help":
                                case "помощь":
                                case "помоги":
                                    {
                                        HandleHelpMessage(userID.ToString(), currentCommand.keyboard);
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
                                        HandleHelpMessage(userID.ToString(), currentCommand.keyboard);
                                        break;
                                    }
                                default:
                                    {
                                        if (messageHandler.GetGroup(messageText.ToUpper(), userID.ToString()))
                                        {
                                            currentCommand.RaiseState();
                                            peopleParser.ChangeStateForUser(userID.ToString());
                                            SendMessage(userID, new MessageResponse(ResponseType.SucceessfulRegistration).response);
                                        }
                                        else
                                        {
                                            SendMessage(userID, new MessageResponse(ResponseType.GroupError).response);
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
                                        messageHandler.GetScheduleForToday(userID.ToString());
                                        break;
                                    }
                                case "расписание на завтра":
                                    {
                                        messageHandler.GetScheduleForNextDay(userID.ToString());
                                        break;
                                    }
                                case "я в столовой":
                                    {
                                        var visitorsCount = messageHandler.GetDinigRoom(userID.ToString());
                                        var text = new MessageResponse(ResponseType.DiningRoom).response;
                                        SendMessage(userID, text + visitorsCount);
                                        break;
                                    }
                                case "ссылки на учебные чаты":
                                    {
                                        messageHandler.GetLinks(userID.ToString());
                                        break;
                                    }
                                case "help":
                                case "/help":
                                case "помощь":
                                case "помоги":
                                    {
                                        HandleHelpMessage(userID.ToString(), currentCommand.keyboard);
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
                SendMessage(userID, text);
            }
        }

        private CommandVK DefineCommand(string chatID)
        {
            var userState = peopleParser.GetStateFromId(chatID);
            if (userState == "")
            {
                userState = "0";
            }
            return new CommandVK(int.Parse(userState));
        }

        private bool IsCorrectCommand(CommandVK currentCommand, string messageText)
        {
            if (availableСommands.Contains(messageText))
            {
                switch (currentCommand.userState)
                {
                    case UsersStates.NotRegister when !currentCommand.availableСommands.Contains(messageText):
                        SendMessage(userID, new MessageResponse(ResponseType.NotRegisterError).response);
                        return false;
                    case UsersStates.RegisterInProcess when !currentCommand.availableСommands.Contains(messageText):
                        SendMessage(userID, new MessageResponse(ResponseType.RegisterInProgressError).response);
                        return false;
                    case UsersStates.Register when !currentCommand.availableСommands.Contains(messageText):
                        SendMessage(userID, new MessageResponse(ResponseType.RegisterError).response);
                        return false;
                    default:
                        return true;
                }
            }

            SendMessage(userID, new MessageResponse(ResponseType.Error).response);
            return false;
        }

        public void SendMessage(long id, string text)
        {
            try
            {
                var currentCommand = DefineCommand(id.ToString());
                vkApi.Messages.Send(new MessagesSendParams
                {
                    UserId = id,
                    Message = text,
                    RandomId = rnd.Next(),
                    Keyboard = currentCommand.keyboard
                });
            }
            
            catch (CannotSendToUserFirstlyException)
            {
                return;
            }
        }

        private void HandleHelpMessage(string chatId, MessageKeyboard keyboard)
        {
            var text = new MessageResponse(ResponseType.Help).response;
            SendMessage(userID, text);
        }

        private void Eye()
        {
            var poll = vkApi.Messages.GetLongPollServer();
            StartAsync(poll.Pts);
            NewMessages += OnMessageReceived;
        }

        private void OnMessageReceived(VkApi owner, ReadOnlyCollection<Message> messages)
        {
            foreach (var message in messages)
            {
                if (message.Type == MessageType.Sended) continue;
                userID = message.FromId.Value;
                Console.Beep();
                Answer(message.Text);
            }
        }

        private static LongPollServerResponse GetLongPoolServer(ulong? lastPts = null)
        {
            var response = vkApi.Messages.GetLongPollServer(lastPts == null);
            ts = ulong.Parse(response.Ts);
            pts = pts == null ? response.Pts : lastPts;
            return response;
        }

        private static Task<LongPollServerResponse> GetLongPoolServerAsync(ulong? lastPts = null)
        {
            return Task.Run(() => GetLongPoolServer(lastPts));
        }

        private static LongPollHistoryResponse GetLongPoolHistory()
        {
            if (!ts.HasValue) GetLongPoolServer(null);
            var pollHistoryParams = new MessagesGetLongPollHistoryParams { Ts = ts.Value, Pts = pts };
            var i = 0;
            LongPollHistoryResponse history = null;
            var errorLog = "";

            while (i < 5 && history == null)
            {
                i++;
                try
                {
                    history = vkApi.Messages.GetLongPollHistory(pollHistoryParams);
                }
                catch (TooManyRequestsException)
                {
                    Thread.Sleep(150);
                    i--;
                }
                catch (Exception ex)
                {
                    errorLog += $"{i} - {ex.Message}{Environment.NewLine}";
                }
            }

            if (history == null) return history;
            pts = history.NewPts;
            foreach (var message in history.Messages)
            {
                message.FromId = message.Type == MessageType.Sended ? vkApi.UserId : message.FromId;
            }
            return history;
        }

        private static Task<LongPollHistoryResponse> GetLongPoolHistoryAsync()
        {
            return Task.Run(GetLongPoolHistory);
        }

        private static async void WatchAsync(object state)
        {
            var history = await GetLongPoolHistoryAsync();
            if (history.Messages.Count > 0)
            {
                currentSleepSteps = 1;
                NewMessages?.Invoke(vkApi, history.Messages);
            }
            else if (currentSleepSteps < maxSleepSteps) currentSleepSteps++;
            watchTimer.Change(currentSleepSteps * stepSleepTime, Timeout.Infinite);
        }

        private static async void StartAsync(ulong? lastPts = null)
        {
            await GetLongPoolServerAsync(lastPts);
            watchTimer = new Timer(WatchAsync, null, 0, Timeout.Infinite);
        }
    }
}
