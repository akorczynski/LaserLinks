namespace DeBouncer
{
    public interface IChatClient
    {
        void Connect(string authCode, int defaultRoom, string defaultFrom);
        void SendMessage(string message, int roomId);
        void SendMessage(string message, int roomId, string from);
    }
}
