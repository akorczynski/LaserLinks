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
        private string _DefaultFrom;

        public void Connect(string authCode, string defaultFrom)
        {
            _DefaultFrom = defaultFrom;
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
