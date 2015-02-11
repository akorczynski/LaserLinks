namespace DeBouncer
{
    public static class ChatFactory
    {
        public static IChatClient GetChatClient(int apiNumber)
        {
            IChatClient result;
            switch (apiNumber)
            {
                case 1:
                    result = new HipChatApi1Wrapper();
                    break;
                case 2:
                    result = new HipChatApi2Wrapper();
                    break;
                default:
                    result = new HipChatApi2Wrapper();
                    break;
            }
            return result;
        }
    }
}
