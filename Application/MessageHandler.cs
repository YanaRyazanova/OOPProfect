﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Functions;
using Infrastructure;
using Infrastructure.SQL;
using Infrastructure.CSV;
using Ninject;

namespace Application
{
    public class MessageHandler
    {
        private readonly DataBaseParserSQL dataBaseParserSql;
        private readonly DataBaseParserCSV dataBaseParserCsv;

        private readonly PeopleParserSQL peopleParserSql;
        private readonly PeopleParserCSV peopleParserCsv;

        private readonly LessonReminder lessonReminder;
        private readonly DiningRoomIndicator diningRoom;
        private string groupName;

        public MessageHandler(
            DiningRoomIndicator diningRoom,
            DataBaseParserSQL dataBaseParserSql,
            DataBaseParserCSV dataBaseParserCsv,
            LessonReminder lessonReminder,
            PeopleParserSQL peopleParserSql,
            PeopleParserCSV peopleparserCsv)
        {
            this.diningRoom = diningRoom;
            this.dataBaseParserSql = dataBaseParserSql;
            this.dataBaseParserCsv = dataBaseParserCsv;
            this.peopleParserSql = peopleParserSql;
            this.peopleParserCsv = peopleparserCsv;
            this.lessonReminder = lessonReminder;

        }

        public string LessonReminderHandler(string group)
        {
            if (group == null)
                return null;
            //var startTime = DataBaseSQL.GetNearestLesson(group);
            var startTime = CSVDataBase.
            var result = Task.Run(() => lessonReminder.Do(startTime.time, startTime.name));
            Console.WriteLine(result.Result);
            return result.Result;
        }

        public string GetResponse(MessageRequest message)
        {
            string result = null;
            //groupName = peopleParserSql.GetGroupFromId(message.userId.ToString());
            groupName = peopleParserCsv.GetGroupFromId(message.userId.ToString());
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
                    diningRoom.Increment(groupName);
                    result = new MessageResponse(ResponseType.DiningRoom).response + diningRoom.VisitorsCount;
                    break;
                default:
                    result = new MessageResponse(ResponseType.Error).response;
                    break;
            }
            return result;
        }

        private string SheduleModify(int days)
        {
            //var scheduleArray = dataBaseParserSql
            //    .GetTimetableForGroupForCurrentDay(groupName, DateTime.Today.AddDays(days));
            var scheduleArray = dataBaseParserCsv
                .GetTimetableForGroupForCurrentDay(groupName, DateTime.Today.AddDays(days));
            var scheduleNextDay = new StringBuilder();
            foreach (var item in scheduleArray)
            {
                scheduleNextDay.Append(item.ToString());
                scheduleNextDay.Append("\n");
            }
            if (scheduleNextDay.Length == 0)
                return "У вас сегодня нет пар в этот день, отдыхайте!";
            return scheduleNextDay.ToString();
        }
    }
}

