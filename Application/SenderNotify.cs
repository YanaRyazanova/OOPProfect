using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Functions;
using Infrastructure;
using Infrastructure.Csv;
using Infrastructure.SQL;

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
                    if (!usersLastNotify.ContainsKey(user))
                    {
                        usersLastNotify[user] = DateTime.Now;
                        flag = true;
                    }

                    var group = peopleParser.GetGroupFromId(user.UserId);
                    var lesson = LessonReminderHandler(group);
                    var difference = DateTime.Now.Hour + DateTime.Now.Minute - usersLastNotify[user].Minute -
                                     usersLastNotify[user].Hour;
                    if (lesson == null ||  difference//40
                        < 3 && !flag)
                        continue;
                    usersLastNotify[user] = DateTime.Now;
                    Console.Write(lesson + user.UserId);
                    //var text = $"{message.LessonName} через {message.TimeToStart} минут";
                    var info = new Tuple<string, int>(lesson.LessonName, lesson.TimeToStart);
                    var result = new LessonReply(info);
                    OnReply(user, result);
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
            var startTime = dataBaseParser.GetNearestLesson(group);
            var result = Task.Run(() => lessonReminder.Do(startTime.time, startTime.name));
            return result.Result;
        }
    }
}
