using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace View
{
    public class CommandVK
    {
        public UsersStates userState;
        public MessageKeyboard keyboard;
        public readonly List<string> availableСommands;
        private static MessageKeyboard CreateGroupKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "ФТ-201",
                        Type = KeyboardButtonActionType.Text
                    },

                    Color = KeyboardButtonColor.Primary
                },
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "ФТ-202",
                        Type = KeyboardButtonActionType.Text
                    },

                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        private static MessageKeyboard CreateMenuKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Расписание на сегодня", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                },
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Расписание на завтра", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            var line2 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Я в столовой", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                },
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Ссылки на учебные чаты", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                },
            };
            var line3 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Help", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            buttonsList.Add(line2);
            buttonsList.Add(line3);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        private static MessageKeyboard CreateStartKeyboard()
        {
            var keyboard = new MessageKeyboard();
            var buttonsList = new List<List<MessageKeyboardButton>>();
            var line1 = new List<MessageKeyboardButton>
            {
                new MessageKeyboardButton
                {
                    Action = new MessageKeyboardButtonAction
                    {
                        Label = "Начать", Type = KeyboardButtonActionType.Text
                    },
                    Color = KeyboardButtonColor.Primary
                }
            };
            buttonsList.Add(line1);
            keyboard.Buttons = buttonsList;
            return keyboard;
        }

        public CommandVK(int userState)
        {
            switch (userState)
            {
                case 0:
                    availableСommands = new List<string> { "/start", "start", "начать", "help", "/help", "помощь", "помоги" };
                    keyboard = CreateStartKeyboard();
                    break;
                case 1:
                    this.userState = UsersStates.RegisterInProcess;
                    availableСommands = new List<string> { "фт-201", "фт-202", "help", "/help", "помощь", "помоги" };
                    keyboard = CreateGroupKeyboard();
                    break;
                case 2:
                    this.userState = UsersStates.Register;
                    availableСommands = new List<string> {"расписание на сегодня", "расписание на завтра", "я в столовой", "ссылки на учебные чаты", "help", "/help", "помощь", "помоги"};
                    keyboard = CreateMenuKeyboard();
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
