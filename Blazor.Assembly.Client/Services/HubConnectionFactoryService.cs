using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blazor.Assembly.Client.Services
{
    public class HubConnectionFactoryService
    {
        private HubConnection _hubConnection;

        public async Task<HubConnection> CreateAsync(string roomname = null)
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                        .WithUrl($"https://localhost:5001/chathub?roomname={roomname}", options =>
                        {
                            options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling; // Allow multiple transports
                        })
                        .WithAutomaticReconnect()
                        .Build();
            }

            return _hubConnection;
        }
    }
}
