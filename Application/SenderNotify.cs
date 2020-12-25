using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Functions;
using Infrastructure;
using Infrastructure.Csv;
using Infrastructure.SQL;

namespace Application
{
    public class SenderNotify
    {

        private Dictionary<string, DateTime> usersLastNotify = new Dictionary<string, DateTime>();

        private readonly IPeopleParser peopleParser;
        private readonly DiningRoomIndicator indicator;
        private readonly IDataBaseParser dataBaseParser;

        private readonly LessonReminder lessonReminder;

        public event Action<string, string> OnReply;
        public event Action<long, string> OnReplyVK;
        public SenderNotify
        (
            LessonReminder lessonReminder,
            IPeopleParser peopleParser,
            DiningRoomIndicator indicator,
            IDataBaseParser dataBaseParser)
        {
            this.peopleParser = peopleParser;
            this.indicator = indicator;
            this.dataBaseParser = dataBaseParser;
            this.lessonReminder = lessonReminder;
        }

        public void Do()
        {
            try
            {
                Console.WriteLine("Hello from BotNotification");
                var usersList = peopleParser.GetAllUsers();
                foreach (var id in usersList)
                {
                    indicator.Decrement(id);
                    var flag = false;
                    if (!usersLastNotify.ContainsKey(id))
                    {
                        usersLastNotify[id] = DateTime.Now;
                        flag = true;
                    }

                    var group = peopleParser.GetGroupFromId(id);
                    var message = LessonReminderHandler(group);
                    if (message == null || (DateTime.Now.Minute - usersLastNotify[id].Minute //40
                        < 3 && !flag))
                        continue;
                    if (message.Contains("пар больше нет"))
                        continue;
                    usersLastNotify[id] = DateTime.Now;
                    Console.Write(message, id);
                    OnReply(id, message);
                    OnReplyVK(long.Parse(id), message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string LessonReminderHandler(string group)
        {
            if (group == null)
                return null;
            var startTime = dataBaseParser.GetNearestLesson(group);
            var result = Task.Run(() => lessonReminder.Do(startTime.time, startTime.name));
            return result.Result;
        }
    }
}
