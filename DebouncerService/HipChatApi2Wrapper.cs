using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipchatApiV2;

namespace DeBouncer
{
    public class HipChatApi2Wrapper : IChatClient
    {
        private HipchatClient _HipchatClient;

        public void Connect(string authCode, int defaultRoom, string defaultFrom)
        {
            _HipchatClient = new HipchatClient(authCode);
        }

        public void SendMessage(string message, int roomID)
        {
            _HipchatClient.SendNotification(roomID, message);
        }

        public void SendMessage(string message, int roomID, string from)
        {
            SendMessage(message, roomID);
        }
    }
}
