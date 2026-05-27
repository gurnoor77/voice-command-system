using Microsoft.AspNetCore.SignalR;

namespace VoiceCommandAPI.Hubs
{
    public class CommandHub : Hub
    {
        public async Task SendCommand(string commandText)
        {
            await Clients.All.SendAsync("ReceiveCommand", commandText);
        }
    }
}