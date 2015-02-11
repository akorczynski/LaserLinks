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

        public void SendMessage(string message, int roomId)
        {
            _HipchatClient.SendNotification(roomId, message);
        }

        public void SendMessage(string message, int roomId, string from)
        {
            SendMessage(message, roomId);
        }
    }
}
