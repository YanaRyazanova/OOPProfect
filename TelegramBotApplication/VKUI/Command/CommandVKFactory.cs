using System;
using System.Collections.Generic;
using System.Text;
using Application;
using Ninject;
using Infrastructure;

namespace View
{
    public class CommandVKFactory
    {
        private readonly StandardKernel container;

        public CommandVKFactory(StandardKernel container)
        {
            this.container = container;
        }

        public CommandVK Create(int userState)
        {
            return userState switch
            {
                0 => container.Get<NotRegisterVK>(),
                1 => container.Get<RegisterInProcessVK>(),
                2 => container.Get<RegisterVK>(),
                3 => container.Get<AddingLinkVK>(),
                _ => throw new Exception("Wrong user state")
            };
        }
    }
}
