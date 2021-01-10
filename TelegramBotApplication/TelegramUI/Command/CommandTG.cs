using Application;
using Telegram.Bot.Types.ReplyMarkups;

namespace View
{
    public interface CommandTG
    {
        public ReplyKeyboardMarkup GetKeyboard();
        public void ProcessMessage(string messageText, BotUser user);
    }
}
