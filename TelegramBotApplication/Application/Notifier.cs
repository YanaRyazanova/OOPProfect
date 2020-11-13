using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace BotApp
{
    class Notifier
    {
        private static async void BotNotificationsSender(object sender, MessageEventArgs messageEventArgs)
        {
            foreach (var id in usersList)
            {
                var message = new Bot.Domain.Functions.LessonReminder("ФТ-201").Do();
                if (message != null)
                    await client.SendTextMessageAsync(id, message);
            }
        }
    }
}
