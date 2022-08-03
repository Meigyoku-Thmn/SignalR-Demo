using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Demo.SignalHub
{
    public interface IMessengerClient
    {
        Task ReceiveExport(string message);
        Task ReceiveMessage(string message);
    }

    public class MessengerHub : Hub<IMessengerClient>
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.ReceiveMessage(message + " ABC");
        }
    }
}
