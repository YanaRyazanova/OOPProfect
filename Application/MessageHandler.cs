using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Functions;
using Infrastructure;
using Infrastructure.SQL;
using Infrastructure.Csv;
using Ninject;

namespace Application
{
    public class MessageHandler
    {
        private readonly DataBaseParserSql dataBaseParserSql;
        private readonly DataBaseParserCsv dataBaseParserCsv;

        private readonly PeopleParserSql peopleParserSql;
        private readonly PeopleParserCsv peopleParserCsv;

        private readonly LessonReminder lessonReminder;
        private readonly DiningRoomIndicator diningRoom;
        private string groupName;

        public MessageHandler(
            DiningRoomIndicator diningRoom,
            DataBaseParserSql dataBaseParserSql,
            DataBaseParserCsv dataBaseParserCsv,
            LessonReminder lessonReminder,
            PeopleParserSql peopleParserSql,
            PeopleParserCsv peopleParserCsv)
        {
            this.diningRoom = diningRoom;
            this.dataBaseParserSql = dataBaseParserSql;
            this.dataBaseParserCsv = dataBaseParserCsv;
            this.peopleParserSql = peopleParserSql;
            this.peopleParserCsv = peopleParserCsv;
            this.lessonReminder = lessonReminder;
        }

        //public event Action<long, string> OnReply; 

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

        public string GetResponse(MessageRequest message)
        {
            string result;
            groupName = peopleParserSql.GetGroupFromId(message.userId.ToString());
            //groupName = peopleParserCsv.GetGroupFromId(message.userId.ToString());
            if (groupName == "")
                return new MessageResponse(ResponseType.StartError).response;
            //{
            //    //OnReply(message.userId, new MessageResponse(ResponseType.StartError).response); 
            //    return;
            //}
            
            switch (message.type)
            {
                case MessagesType.ScheduleForToday:
                    var schedule = SheduleModify(0);
                    result = new ScheduleSender(schedule).Do();
                    break;
                case MessagesType.Help:
                    result = new MessageResponse(ResponseType.Help).response;
                    break;
                case MessagesType.ScheduleForTomorrow:
                    var scheduleNextDay = SheduleModify(1);
                    result = new ScheduleSender(scheduleNextDay).Do();
                    break;
                case MessagesType.DiningRoom:
                    diningRoom.Increment(message.userId.ToString());
                    result = new MessageResponse(ResponseType.DiningRoom).response + diningRoom.VisitorsCount;
                    break;
                case MessagesType.Start:
                    result = new MessageResponse(ResponseType.Start).response;
                    break;
                default:
                    result = new MessageResponse(ResponseType.Error).response;
                    break;
            }
            //OnReply(message.userId, result);
            return result;
        }

        private string SheduleModify(int days)
        {
            var scheduleArray = dataBaseParserSql
                .GetTimetableForGroupForCurrentDay(groupName, DateTime.Today.AddDays(days));
            //var scheduleArray = dataBaseParserCsv
            //    .GetTimetableForGroupForCurrentDay(groupName, DateTime.Today.AddDays(days));
            var scheduleNextDay = new StringBuilder();
            foreach (var item in scheduleArray)
            {
                scheduleNextDay.Append(item);
                scheduleNextDay.Append("\n");
            }
            return scheduleNextDay.Length == 0 ? "У вас сегодня нет пар в этот день, отдыхайте!" : scheduleNextDay.ToString();
        }
    }
}

