namespace DeBouncer
{
    public interface IChatClient
    {
        void Connect(string authCode, string defaultFrom);
        void SendMessage(string message, int roomID);
        void SendMessage(string message, int roomID, string from);
    }
}
