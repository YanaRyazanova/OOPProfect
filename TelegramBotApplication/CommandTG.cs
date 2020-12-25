using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public enum UsersStates
    {
        NotRegister,
        Register,
        RegisterInProcess
    }

    public class CommandTG
    {
        public UsersStates userState;
        public ReplyKeyboardMarkup keyboard;
        public readonly List<string> availableСommands;
        private static ReplyKeyboardMarkup CreateKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("Расписание на сегодня"),
                    new KeyboardButton("Расписание на завтра")
                },

                new[]
                {
                    new KeyboardButton("Я в столовой"),
                    new KeyboardButton("Ссылки на учебные чаты")
                },

                new []
                {
                    new KeyboardButton("Help")
                }
            });
            return keyboard;
        }

        private static ReplyKeyboardMarkup CreateStartKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("ФТ-201"),
                    new KeyboardButton("ФТ-202")
                }
            });
            return keyboard;
        }
        private static ReplyKeyboardMarkup CreatePreStartKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new []
                {
                    new KeyboardButton("Начать")
                }
            });
            return keyboard;
        }

        public CommandTG(int userState)
        {
            switch (userState)
            {
                case 0:
                    availableСommands = new List<string> {"/start", "start", "начать", "help", "/help", "помощь", "помоги" };
                    keyboard = CreatePreStartKeyboard();
                    break;
                case 1:
                    this.userState = UsersStates.RegisterInProcess;
                    availableСommands = new List<string> {"фт-201", "фт-202", "help", "/help", "помощь", "помоги" };
                    keyboard = CreateStartKeyboard();
                    break;
                case 2:
                    this.userState = UsersStates.Register;
                    availableСommands = new List<string>
                        {"расписание на сегодня", "расписание на завтра", "я в столовой", "ссылки на учебные чаты", "help", "/help", "помощь", "помоги"};
                    keyboard = CreateKeyboard();
                    break;
            }
        }

        public void RaiseState()
        {
            switch (userState)
            {
                case UsersStates.NotRegister:
                    userState = UsersStates.RegisterInProcess;
                    break;
                case UsersStates.RegisterInProcess:
                    userState = UsersStates.Register;
                    break;
            }
        }
    }
}
