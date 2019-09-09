using Microsoft.AspNetCore.SignalR;

namespace TopWords.Hubs
{
    public class TopWordsHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
