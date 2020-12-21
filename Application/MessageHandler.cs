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

        private readonly IDataBaseParser dataBaseParser;
        private readonly DataBaseParserCsv dataBaseParserCsv;

        private readonly IPeopleParser peopleParser;
        private readonly ILinkParser linkParser;

        private readonly DiningRoomIndicator diningRoom;
        

        public MessageHandler(
            DiningRoomIndicator diningRoom,
            IDataBaseParser dataBaseParser,
            IPeopleParser peopleParser,
            SenderNotify senderNotify,
            ILinkParser linkParser)
        {
            this.senderNotify = senderNotify;
            this.diningRoom = diningRoom;
            this.dataBaseParser = dataBaseParser;
            this.peopleParser = peopleParser;
            this.linkParser = linkParser;
        }

        public event Action<string, string> OnReply;
        public event Action<long, string> OnReplyVK;

        public void Run()
        {
            var tm = new TimerCallback(Method);
            var timer = new Timer(tm, null, 0, 120000);
        }

        private void Method(object obj)
        {
            senderNotify.Do();
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
            //peopleParser.AddNewUser(userId.ToString(), group, "0");
            peopleParser.ChangeGroup(userId.ToString(), group);
        }

        public void GetLinks(string userId)
        {
            var group = peopleParser.GetGroupFromId(userId);
            var result = linkParser.GetActualLinksForGroup(group);
            OnReply(userId, result.ToString());
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

