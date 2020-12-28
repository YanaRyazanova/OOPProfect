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

    public abstract class CommandTG
    {
        public UsersStates userState;

        public abstract ReplyKeyboardMarkup GetKeyboard();
        public abstract void ProcessMessage(string messageText, TGUser chatId);

        public CommandTG(UsersStates userState)
        {
            this.userState = userState;
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
