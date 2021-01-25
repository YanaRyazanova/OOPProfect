using System;
using System.Text;
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
            var user = new BotUser(message.Chat.Id.ToString(), "tg");
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
        
        public void SendMessage(BotUser user, Reply reply)
        {
            var currentCommand = DefineCommand(user);
            var message = reply switch
            {
                ScheduleReply s => GetReply(s),
                LessonReply l => GetLessonReply(l),
                LinksReply s => GetLinksReply(s),
                _ => null
            };
            if (message != null) tgMessageSender.SendNotification(user, message, currentCommand.GetKeyboard());
        }

        private static string GetLinksReply(LinksReply links)
        {
            var result = new StringBuilder();
            foreach (var link in links.links)
            {
                result.Append($"{link.name}: {link.link}");
                result.Append("\n");
            }

            return result.ToString();
        }

        private static string GetLessonReply(LessonReply lessonReply)
        {
            if (lessonReply == null)
                return null;
            var reply = $"{lessonReply.LessonInfo.Item1} через {lessonReply.LessonInfo.Item2} минут";
            var text = new MessageResponse(ResponseType.NextLesson).response;
            return text + reply;
        }

        private static string GetReply(ScheduleReply reply)
        {
            if (reply.lessons.Length == 0)
                return "Сегодня пар больше нет :-)";
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