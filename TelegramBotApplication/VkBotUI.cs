using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Domain;
using Infrastructure.Csv;
using Infrastructure.SQL;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using ResponseType = Application.ResponseType;

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
        private static PeopleParserSql peopleParserSql;
        private static PeopleParserCsv peopleParserCsv;
        private Dictionary<string, DateTime> usersLastNotify = new Dictionary<string, DateTime>();
        private static Random rnd = new Random();

        public VkBotUI(VkApi api, string keyVkToken, MessageHandler handler, PeopleParserSql newPeopleParserSql, PeopleParserCsv newPeopleParserCsv)
        {
            vkApi = api;
            vkToken = keyVkToken;
            messageHandler = handler;
            peopleParserSql = newPeopleParserSql;
            peopleParserCsv = newPeopleParserCsv;
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

        private static void Answer(string message)
        {
            string text;
            MessageKeyboard keyboard;
            try
            {
                switch (message)
                {
                    case "/start":
                    case "Начать":
                    case "Start":
                        text = new MessageResponse(ResponseType.Start).response;
                        keyboard = CreateGroupKeyboard();
                        break;
                    case "ФТ-201":
                    case "ФТ-202":
                        peopleParserSql.AddNewUser(userID.ToString(), message);
                        text = "Выберите пункт меню";
                        keyboard = CreateMenuKeyboard();
                        break;
                    default:
                        //text = messageHandler.GetResponse(new MessageRequest(message.ToLower(), userID));
                        keyboard = CreateMenuKeyboard();
                        break;
                }
            }
            catch (Exception e)
            {
                if (e.Message == "constraint failed\r\nUNIQUE constraint failed: PeopleAndGroups.ChatID")
                {
                    text = "Вы уже зарегистрированы в боте. Смену группы мы добавим позже :-)";
                    keyboard = CreateMenuKeyboard();
                }
                else
                {
                    text = "Упс! Кажется что-то пошло не так!Попробуйте начать с команды '/start'";
                    keyboard = CreateStartKeyboard();
                    Console.WriteLine(e);
                }
            }
            //SendMessage(userID, text, keyboard);
        }

        public static void SendMessage(long id, string text, MessageKeyboard keyboard)
        {
            vkApi.Messages.Send(new MessagesSendParams
            {
                UserId = id,
                Message = text,
                RandomId = rnd.Next(),
                Keyboard = keyboard
            });
        }

        private static MessageKeyboard CreateStartKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Начать", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        private static MessageKeyboard CreateGroupKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "ФТ-201",
                        Type = KeyboardButtonActionType.Text
                    },

                    Color = KeyboardButtonColor.Primary
                },
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "ФТ-202",
                        Type = KeyboardButtonActionType.Text
                    },

                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        private static MessageKeyboard CreateMenuKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Расписание на сегодня", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Positive
                },
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Расписание на завтра", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            var line2 = new List<MessageKeyboardButton>();
            line2.Add(new MessageKeyboardButton
            {
                Action = new MessageKeyboardButtonAction
                {
                    Label = "Я в столовой",
                    Type = KeyboardButtonActionType.Text
                },

                Color = KeyboardButtonColor.Primary
            });
            line2.Add(new MessageKeyboardButton
            {
                Action = new MessageKeyboardButtonAction
                {
                    Label = "Help",
                    Type = KeyboardButtonActionType.Text
                },

                Color = KeyboardButtonColor.Primary
            });
            buttonsList.Add(line1);
            buttonsList.Add(line2);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        //public void BotNotificationSender()
        //{
        //    var usersList = peopleParserSql.GetAllUsers();
        //    //var usersList = peopleParserCsv.GetAllUsers();
        //    foreach (var id in usersList)
        //    {
        //        var flag = false;
        //        if (!usersLastNotify.ContainsKey(id))
        //        {
        //            usersLastNotify[id] = DateTime.Now;
        //            flag = true;
        //        }
        //        var group = peopleParserSql.GetGroupFromId(id);
        //        //var group = peopleParserCsv.GetGroupFromId(id);
        //        var message = messageHandler.LessonReminderHandler(group);
        //        if (message == null || (DateTime.Now.Minute - usersLastNotify[id].Minute < //87
        //            5 && !flag))
        //            continue;
        //        if (message.Contains("пар больше нет"))
        //            continue;
        //        try
        //        {
        //            usersLastNotify[id] = DateTime.Now;
        //            SendMessage(long.Parse(id), message, CreateMenuKeyboard());
        //        }
        //        catch
        //        {
        //            return;
        //        }
        //    }
        //}

        private static void Eye()
        {
            var poll = vkApi.Messages.GetLongPollServer();
            StartAsync(poll.Pts);
            NewMessages += OnMessageReceived;
        }

        private static void OnMessageReceived(VkApi owner, ReadOnlyCollection<Message> messages)
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
