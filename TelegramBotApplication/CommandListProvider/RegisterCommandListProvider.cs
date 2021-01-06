using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public class RegisterCommandListProvider : CommandListProvider
    {
        private const string DiningRoom = "я в столовой";
        public string GetingDiningRoom => DiningRoom;
        public  List<string> GetCommands()
        {
            return new List<string>
            {
                "расписание на сегодня", "расписание на завтра", DiningRoom, "ссылки на учебные чаты", "help",
                "/help", "помощь", "помоги"
            };
        }
    }
}
