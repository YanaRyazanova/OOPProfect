using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public class RegisterCommandListProvider : CommandListProvider
    {
        public override List<string> GetCommands()
        {
            return new List<string>
            {
                "расписание на сегодня", "расписание на завтра", "я в столовой", "ссылки на учебные чаты", "help",
                "/help", "помощь", "помоги"
            };
        }
    }
}
