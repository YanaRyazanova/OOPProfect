﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Functions;
using Infrastructure;
using Ninject;

namespace Application
{
    public class MessageHandler
    {
        private readonly DataBase dataBase;
        private readonly DiningRoomIndicator diningRoom;
        private readonly DataBaseParser dataBaseParser;

        private readonly PeopleParser peopleParser;
        private readonly LessonReminder lessonReminder;
        private string groupName;

        public MessageHandler(DataBase dataBase, DiningRoomIndicator diningRoom,
            DataBaseParser dataBaseParser,
            LessonReminder lessonReminder,
            PeopleParser peopleParser)
        {
            this.dataBase = dataBase;
            this.diningRoom = diningRoom;
            this.dataBaseParser = dataBaseParser;
            this.peopleParser = peopleParser;
            this.lessonReminder = lessonReminder;
            
        }

        public string LessonReminderHandler(string group)
        {
            group = groupName;
            if (groupName == null)
                return null;
            //var startTime = DataBase.GetNearestLesson(group);
            //var result = Task.Run(() => lessonReminder.Do(startTime.time, startTime.name));
            var result = Task.Run(() => lessonReminder.Do(DateTime.Now.AddMinutes(3), "Самая лучшая пара в твоей жизни"));
            return result.Result;
        }

        public string GetResponse(Messages message)
        {
            string result = null;
            groupName = peopleParser.GetGroupFromId(message.userId.ToString());
            switch (message.type)
            {
                case MessagesType.ScheduleForToday:
                    var schedule = SheduleModify(0);
                    result = new ScheduleSender(schedule).Do();
                    break;
                case MessagesType.Help:
                    result = "Бот умеет вот это";
                    break;
                case MessagesType.ScheduleForTomorrow:
                    var scheduleNextDay = SheduleModify(1);
                    result = new ScheduleSender(scheduleNextDay).Do();
                    //result = "Hello everyone";
                    break;
                case MessagesType.DiningRoom:
                    diningRoom.Increment();
                    result = $"Сейчас в столовой {diningRoom.VisitorsCount} посетителей :-)";
                    break;
                default:
                    result = "К сожалению, бот не обрабатывает такую команду :-(";
                    break;
            }
            return result;
        }

        private string SheduleModify(int days)
        {
            var scheduleArray = dataBaseParser
                .GetTimetableForGroupForCurrentDay(groupName, DateTime.Today.AddDays(days));
            var scheduleNextDay = new StringBuilder();
            foreach (var item in scheduleArray)
            {
                scheduleNextDay.Append(item.ToString());
                scheduleNextDay.Append("\n");
            }

            return scheduleNextDay.ToString();
        }
    }
}

