using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        //private readonly LessonReminder<Lesson> lessonReminder;
        private readonly string groupName;

        public MessageHandler(DataBase dataBase, DiningRoomIndicator diningRoom,
            DataBaseParser dataBaseParser,
            //LessonReminder<Lesson> lessonReminder,
            string groupName)
        {
            this.dataBase = dataBase;
            this.diningRoom = diningRoom;
            this.dataBaseParser = dataBaseParser;
            //this.lessonReminder = lessonReminder;
            this.groupName = groupName;
        }

        private void LessonReminderHandler(Func<string, string> schedule)
        {
            //??????????????var lessonReminderMessage = Task.Run(() => lessonReminder.Do());
        }

        public string GetResponse(string message)
        {
            string result = null;
            switch (message)
            {
                case "/start":
                    result = "Hello";
                    break;
                case "расписание на сегодня":
                    var schedule = SheduleModify(0);
                    result = new ScheduleSender(schedule).Do();
                    break;
                case "help":
                    result = "Бот умеет вот это";
                    break;
                case "расписание на завтра":
                    var scheduleNextDay = SheduleModify(1);
                    result = new ScheduleSender(scheduleNextDay).Do();
                    //result = "Hello everyone";
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

