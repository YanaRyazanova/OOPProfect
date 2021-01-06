using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Infrastructure;
using VkNet;
using VkNet.Enums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace View
{
    public class VkBotUI
    {
        private static VkApi vkApi;
        private static string vkToken;
        private static BotUser user;
        private static ulong? ts;
        private static ulong? pts;
        private static Timer watchTimer = null;
        private const byte maxSleepSteps = 3;
        private const int stepSleepTime = 333;
        private static byte currentSleepSteps = 1;
        private delegate void MessagesRecievedDelegate(VkApi owner, ReadOnlyCollection<Message> messages);
        private static event MessagesRecievedDelegate NewMessages;
        private static IPeopleParser peopleParser;
        private readonly CommandVKFactory commandVkFactory;
        private readonly VKMessageSender vkMessageSender;
        public VkBotUI(VkApi api,
            string keyVkToken,
            VKMessageSender vkMessageSender,
            CommandVKFactory commandVkFactory,
            IPeopleParser newPeopleParser)
        {
            vkApi = api;
            vkToken = keyVkToken;
            this.vkMessageSender = vkMessageSender;
            this.commandVkFactory = commandVkFactory;
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
            Console.WriteLine(messageText);
            var vkUser = user;
            var currentCommand = DefineCommand(user.UserId);
            try
            {
                currentCommand.ProcessMessage(messageText, vkUser);
            }

            catch (Exception e)
            {
                var text = new MessageResponse(ResponseType.CatchError).response;
                Console.WriteLine(e);
                vkMessageSender.SendNotification(vkUser, text, currentCommand.GetKeyboard());
            }
        }

        private CommandVK DefineCommand(string chatID)
        {
            var userState = peopleParser.GetStateFromId(chatID);
            if (userState == "")
            {
                userState = "0";
            }
            return commandVkFactory.Create(int.Parse(userState));
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
                user = new BotUser(message.FromId.Value.ToString(), "vk");
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

        private CommandVK DefineCommand(BotUser user)
        {
            var userState = peopleParser.GetStateFromId(user.UserId);
            if (userState == "")
            {
                userState = "0";
            }
            return commandVkFactory.Create(int.Parse(userState));
        }

        public void SendMessage(BotUser user, Application.Reply reply)
        {
            var currentCommand = DefineCommand(user);
            var message = (reply) switch
            {
                ScheduleReply s => GetReply(s),
            };
            vkMessageSender.SendNotification(user, message, currentCommand.GetKeyboard());
        }
        private static string GetReply(ScheduleReply reply)
        {
            var scheduleNextDay = new StringBuilder();
            foreach (var item in reply.lessons)
            {
                scheduleNextDay.Append(item);
                scheduleNextDay.Append("\n");
            }

            return scheduleNextDay.Length == 0 ? null : scheduleNextDay.ToString();
        }
    }
}
