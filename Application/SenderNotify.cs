using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Functions;
using Infrastructure.Csv;
using Infrastructure.SQL;

namespace Application
{
    public class SenderNotify
    {

        private Dictionary<string, DateTime> usersLastNotify = new Dictionary<string, DateTime>();

        private readonly PeopleParserSql peopleParserSql;
        private readonly PeopleParserCsv peopleParserCsv;

        private readonly LessonReminder lessonReminder;

        public event Action<string, string> OnReply;
        public event Action<long, string> OnReplyVK;
        public SenderNotify
        (
            LessonReminder lessonReminder,
            PeopleParserSql peopleParserSql,
            PeopleParserCsv peopleParserCsv)
        {
            this.peopleParserSql = peopleParserSql;
            this.peopleParserCsv = peopleParserCsv;
            this.lessonReminder = lessonReminder;
        }

        public void Do()
        {
            try
            {
                Console.WriteLine("Hello from BotNotification");
                var usersList = peopleParserSql.GetAllUsers();
                //var usersList = peopleParserCsv.GetAllUsers();
                foreach (var id in usersList)
                {
                    var flag = false;
                    if (!usersLastNotify.ContainsKey(id))
                    {
                        usersLastNotify[id] = DateTime.Now;
                        flag = true;
                    }

                    var group = peopleParserSql.GetGroupFromId(id);
                    //var group = peopleParserCsv.GetGroupFromId(id);
                    var message = LessonReminderHandler(group);
                    if (message == null || (DateTime.Now.Minute - usersLastNotify[id].Minute //40
                        < 3 && !flag))
                        continue;
                    if (message.Contains("пар больше нет"))
                        continue;
                    usersLastNotify[id] = DateTime.Now;
                    Console.Write(message, id);
                    OnReply(id, message);
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
            var startTime = DataBaseSql.GetNearestLesson(group);
            //var startTime = dataBaseParserCsv.GetNearestLesson(group);
            var result = Task.Run(() => lessonReminder.Do(startTime.time, startTime.name));
            //var result = Task.Run(() => lessonReminder.Do(DateTime.Now.AddMinutes(7), "Самая лучшая пара в твоей жизни"));
            return result.Result;
        }
    }
}
