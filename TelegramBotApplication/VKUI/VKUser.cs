using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public class VKUser
    {
        public VKUser(long userId)
        {
            userID = userId;
        }

        public long userID { get; }
    }
}
