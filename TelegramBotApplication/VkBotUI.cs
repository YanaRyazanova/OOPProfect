using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Application;
using VkNet;
using VkNet.Enums;
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

        public VkBotUI(VkApi api, string keyVkToken, MessageHandler handler)
        {
            vkApi = api;
            vkToken = keyVkToken;
            messageHandler = handler;
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
            //Console.ReadLine();
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
            message = message.ToLower();
            var text = messageHandler.GetResponse(message);
            vkApi.Messages.Send(new MessagesSendParams
            {
                UserId = userID,
                Message = text,
                RandomId = text.GetHashCode()
            });
        }

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
                //Console.Beep();
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
