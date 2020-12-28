using System;
using System.Collections.Generic;
using System.Text;

namespace View
{
    public class TGUser
    {
        public TGUser(long userId)
        {
            userID = userId;
        }

        public long userID { get; }
    }
}
