using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Telegram.Bot.Types.ReplyMarkups;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public enum VkUsersStates
    {
        NotRegister,
        Register,
        RegisterInProcess
    }


    public abstract class CommandVK
    {
        public VkUsersStates VkUserState;

        public abstract MessageKeyboard GetKeyboard();
        public abstract void ProcessMessage(string messageText, BotUser user);

        public CommandVK(VkUsersStates vkUserState)
        {
            this.VkUserState = vkUserState;
        }

        public void RaiseState()
        {
            switch (VkUserState)
            {
                case VkUsersStates.NotRegister:
                    VkUserState = VkUsersStates.RegisterInProcess;
                    break;
                case VkUsersStates.RegisterInProcess:
                    VkUserState = VkUsersStates.Register;
                    break;
            }
        }
    }
}
