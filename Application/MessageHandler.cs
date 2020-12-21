using System;
using System.Collections.Generic;
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
        private readonly SenderNotify senderNotify;

        private static List<DateTime> times = new List<DateTime>
        {
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 50, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 30, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 40, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 20, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 00, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 40, 0),
        };

        private readonly IDataBaseParser dataBaseParser;
        private readonly DataBaseParserCsv dataBaseParserCsv;

        private readonly IPeopleParser peopleParser;

        private readonly DiningRoomIndicator diningRoom;
        //private string groupName;

        public MessageHandler(
            DiningRoomIndicator diningRoom,
            IDataBaseParser dataBaseParser,
            IPeopleParser peopleParser,
            SenderNotify senderNotify)
        {
            this.senderNotify = senderNotify;
            this.diningRoom = diningRoom;
            this.dataBaseParser = dataBaseParser;
            this.peopleParser = peopleParser;
        }

        public event Action<string, string> OnReply;

        public void Run()
        {
            while (true)
            {
                times.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0));
                var currentTime = DateTime.Now;
                foreach (var time in times)
                {
                    var difference = currentTime.Hour + currentTime.Minute - time.Hour - time.Minute;
                    if (difference >= 2 || difference < 0) continue;
                    senderNotify.Do();
                    //vkBot.BotNotificationSender();
                }
            }
        }

        public void GetScheduleForToday(string userId)
        {
            var schedule = SheduleModify(0, userId);
            OnReply(userId, schedule);
        }

        public void GetScheduleForNextDay(string userId)
        {
            var scheduleNextDay = SheduleModify(1, userId);
            var result = new ScheduleSender(scheduleNextDay).Do();
            OnReply(userId, result);
        }

        public void GetDinigRoom(string userId)
        {
            diningRoom.Increment(userId);
            OnReply(userId, diningRoom.VisitorsCount.ToString());
        }

        public void GetGroup(string group, long userId)
        {
            peopleParser.AddNewUser(userId.ToString(), group);
        }


        private string SheduleModify(int days, string userId)
        {
            var groupName = peopleParser.GetGroupFromId(userId);
            var scheduleArray = dataBaseParser
                .GetTimetableForGroupForCurrentDay(groupName, DateTime.Today.AddDays(days));
            var scheduleNextDay = new StringBuilder();
            foreach (var item in scheduleArray)
            {
                scheduleNextDay.Append(item.ToString());
                scheduleNextDay.Append("\n");
            }

            return scheduleNextDay.Length == 0 ? null : scheduleNextDay.ToString();
        }
    }
}

