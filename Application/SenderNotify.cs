using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Functions;
using Infrastructure;

namespace Application
{
    public class SenderNotify
    {
        private Dictionary<BotUser, DateTime> usersLastNotify = new Dictionary<BotUser, DateTime>();

        private readonly IPeopleParser peopleParser;
        private readonly DiningRoomIndicator indicator;
        private readonly IDataBaseParser dataBaseParser;

        private readonly LessonReminder lessonReminder;

        public event Action<BotUser, Reply> OnReply;
        private object lockObj = new object();
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
                    var domain = peopleParser.GetPlatformFromId(id);
                    var user = new BotUser(id, domain);
                    indicator.Decrement(id);
                    var flag = false;
                    lock (lockObj)
                    {
                        if (!usersLastNotify.ContainsKey(user))
                        {
                            usersLastNotify[user] = DateTime.Now;
                            flag = true;
                        }
                    }
                    var group = peopleParser.GetGroupFromId(user.UserId);
                    var lesson = LessonReminderHandler(group);
                    lock (lockObj)
                    {
                        var lastNotifyDifference = DateTime.Now.Hour + DateTime.Now.Minute - usersLastNotify[user].Minute -
                                                   usersLastNotify[user].Hour;
                        if (lesson == null || lastNotifyDifference//40
                            > 3 && !flag)
                            continue;
                        usersLastNotify[user] = DateTime.Now;
                    }
                    Console.Write(lesson + user.UserId);
                    var info = new Tuple<string, int>(lesson.LessonName, lesson.TimeToStart);
                    var result = new LessonReply(info);
                    OnReply?.Invoke(user, result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public NextLesson LessonReminderHandler(string group)
        {
            if (group == null)
                return null;
            var nearestLesson = dataBaseParser.GetNearestLesson(group);
            if (nearestLesson.name == null)
                return null;
            var result = Task.Run(() => lessonReminder.Do(nearestLesson.time, nearestLesson.name));
            return result.Result;
        }
    }
}
