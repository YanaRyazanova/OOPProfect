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

        private readonly IPeopleParser peopleParser;
        private readonly ILinkParser linkParser;

        private readonly DiningRoomIndicator diningRoom;
        private static Timer timer;
        public List<string> GetAllGroups () => groups;
        private readonly List<string> groups = new List<string> {"ФТ-201", "ФТ-202"};
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
            timer = new Timer(
                callback: new TimerCallback(Method),
                state: null,
                dueTime: 1000,
                period: 10000);
        }

        private void Method(object obj)
        {
            senderNotify.Do();
        }

        public void GetScheduleForToday(string userId)
        {
            var schedule = SheduleModify(0, userId);
            OnReply(userId, schedule);
            OnReplyVK(long.Parse(userId), schedule);
        }

        public void GetScheduleForNextDay(string userId)
        {
            var scheduleNextDay = SheduleModify(1, userId);
            var result = new ScheduleSender(scheduleNextDay).Do();
            OnReply(userId, result);
            OnReplyVK(long.Parse(userId), scheduleNextDay);
        }

        public int GetDinigRoom(string userId)
        {
            diningRoom.Increment(userId);
            return diningRoom.VisitorsCount;
        }

        public bool GetGroup(string group, string userId)
        {
            if (groups.Contains(group))
            {
                peopleParser.ChangeGroup(userId, group);
                return true;
            }

            return false;
        }

        public void GetLinks(string userId)
        {
            var group = peopleParser.GetGroupFromId(userId);
            var links = linkParser.GetActualLinksForGroup(group);
            var result = new StringBuilder();
            foreach (var link in links)
            {
                result.Append($"{link.name}: {link.link}");
                result.Append("\n");
            }
            OnReply(userId, result.ToString());
            OnReplyVK(long.Parse(userId), result.ToString());
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

