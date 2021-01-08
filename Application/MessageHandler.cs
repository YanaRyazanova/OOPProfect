using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        private readonly GroupProvider groupProvider;

        private readonly DiningRoomIndicator diningRoom;
        private Timer timer;
        
        public MessageHandler(
            DiningRoomIndicator diningRoom,
            IDataBaseParser dataBaseParser,
            IPeopleParser peopleParser,
            SenderNotify senderNotify,
            ILinkParser linkParser,
            GroupProvider groupProvider)
        {
            this.senderNotify = senderNotify;
            this.diningRoom = diningRoom;
            this.dataBaseParser = dataBaseParser;
            this.peopleParser = peopleParser;
            this.linkParser = linkParser;
            this.groupProvider = groupProvider;
        }

        public event Action<BotUser, Reply> OnReply;

        public void Run()
        {
            senderNotify.OnReply += OnReply;
            timer = new Timer(
                callback: _ => senderNotify.Do(),
                state: null,
                dueTime: 1000,
                period: 120000);
        }

        public void GetScheduleForToday(BotUser user)
        {
            var schedule = SheduleModify(0, user);
            OnReply(user, schedule);
        }

        public void GetScheduleForNextDay(BotUser user)
        {
            var scheduleNextDay = SheduleModify(1, user);
            OnReply(user, scheduleNextDay);
        }

        public int GetDiningRoom(BotUser user)
        {
            diningRoom.Increment(user.UserId);
            return diningRoom.VisitorsCount;
        }

        public bool SaveGroup(string group, BotUser user)
        {
            if (!groupProvider.GetAllGroups().Contains(group)) return false;
            peopleParser.ChangeGroup(user.UserId, group);
            return true;
        }

        public bool AddLink(BotUser user, string name, string link)
        {
            try
            {
                var group = peopleParser.GetGroupFromId(user.UserId);
                linkParser.AddLinkForGroup(group, name, link);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void GetLinks(BotUser user)
        {
            var group = peopleParser.GetGroupFromId(user.UserId);
            var links = linkParser.GetActualLinksForGroup(group);
            OnReply(user, new LinksReply(links));
        }

        private Reply SheduleModify(int days, BotUser user)
        {
            var groupName = peopleParser.GetGroupFromId(user.UserId);
            var scheduleArray = dataBaseParser
                .GetTimetableForGroupForCurrentDay(groupName, DateTime.Today.AddDays(days));
            return new ScheduleReply(scheduleArray);
        }
    }
}

