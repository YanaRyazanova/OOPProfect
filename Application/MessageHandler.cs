using System;
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
        private readonly ScheduleSender<Lesson> scheduleSender;
        private readonly string groupName;

        public MessageHandler(DataBase dataBase, DiningRoomIndicator diningRoom,
            DataBaseParser dataBaseParser, ScheduleSender<Lesson> scheduleSender)
        {
            this.dataBase = dataBase;
            this.diningRoom = diningRoom;
            this.dataBaseParser = dataBaseParser;
            this.scheduleSender = scheduleSender;
        }

        private void LessonReminderHandlerHandler(Func<string, string> schedule)
        {
            var lessonReminderMessage = Task.Run(() => schedule);
        }

        public string GetResponse(string message)
        {
            string result = null;
            switch (message)
            {
                case "расписание на сегодня":
                    var schedule = dataBaseParser.GetTimetableForGroupForCurrentDay(groupName, DateTime.Today);
                    result = scheduleSender.Do().ToString();
                    break;
                case "help":
                    result = "Бот умеет вот это";
                    break;
                case "расписание на завтра":
                    //var scheduleNextDay =
                    //    dataBaseParser.GetTimetableForGroupForCurrentDay(groupName, DateTime.Today.AddDays(1));
                    //result = scheduleSender.Do().ToString();
                    result = "Hello everyone";
                    break;
                case "я в столовой":
                    diningRoom.Increment();
                    result = $"Сейчас в столовой {diningRoom.VisitorsCount} посетителей :-)";
                    break;
                default:
                    result = "К сожалению, бот не обрабатывает такую команду :-(";
                    break;
            }

            return result;
        }
    }
}

