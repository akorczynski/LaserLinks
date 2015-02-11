using HipChat;

namespace DeBouncer
{
    public class HipChatApi1Wrapper : IChatClient
    {
        private HipChatClient _HipchatClient;

        public void Connect(string authCode, int defaultRoom, string defaultFrom)
        {
            _HipchatClient = new HipChatClient(authCode, defaultRoom, "default");
        }

        public void SendMessage(string message, int roomId)
        {
            _HipchatClient.SendMessage(message, roomId);
        }

        public void SendMessage(string message, int roomId, string from)
        {
            _HipchatClient.SendMessage(message, roomId, from);
        }
    }
}
