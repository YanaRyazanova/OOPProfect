using System;
using View.TelegramUI.UserStates;
using Ninject;

namespace View
{
    public class CommandTGFactory
    {
        private readonly StandardKernel container;

        public CommandTGFactory(StandardKernel container)
        {
            this.container = container;
        }

        public CommandTG Create(int userState)
        {
            return userState switch
            {
                0 => container.Get<NotRegisterTG>(),
                1 => container.Get<RegisterInProcessTG>(),
                2 => container.Get<RegisterTG>(),
                3 => container.Get<AddingLinkTG>(),
                _ => throw new Exception("Wrong user state")
            };
        }
    }
}
