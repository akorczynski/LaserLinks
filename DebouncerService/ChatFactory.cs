using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipchatApiV2;

namespace DeBouncer
{
    public static class ChatFactory
    {
        public static IChatClient GetChatClient()
        {
            return new HipChatApi2Wrapper();
        }
    }
}
