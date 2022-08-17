using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Demo.SignalHub
{
    // By default, SignalR use NameIdentifier (nameid) as User ID, you can provide a custom identifier for it.
    // Remember to register the service.
    public class MessengerUserIdProvider : IUserIdProvider
    {
        // For example: provide unique_name instead
        public string GetUserId(HubConnectionContext connection) => connection.User.Identity.Name;
    }

    public interface IMessengerClient
    {
        Task ReceiveExport(string message);
        Task ReceiveMessage(string message);
    }

    [Authorize]
    public class MessengerHub : Hub<IMessengerClient>
    {
        public async Task SendMessage(string message)
        {
            Console.WriteLine("Message from: " + Context.UserIdentifier);
            await Clients.All.ReceiveMessage(message + " ABC");
        }
    }
}
