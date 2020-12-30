using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public enum TgUsersStates
    {
        NotRegister,
        Register,
        RegisterInProcess
    }

    public abstract class CommandTG
    {
        public TgUsersStates TgUserState;

        public abstract ReplyKeyboardMarkup GetKeyboard();
        public abstract void ProcessMessage(string messageText, BotUser user);

        public CommandTG(TgUsersStates tgUserState)
        {
            this.TgUserState = tgUserState;
        }
    }
}
