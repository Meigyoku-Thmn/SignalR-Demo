using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalR_Demo.SignalHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Demo.Controllers
{
    [ApiController]
    [Route("messenger")]
    public class MessengerController : ControllerBase
    {
        readonly IHubContext<MessengerHub, IMessengerClient> _hubContext;

        public MessengerController(IHubContext<MessengerHub, IMessengerClient> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("send-export-message")]
        public IActionResult SendExportMessage([FromQuery] string userIdentifier)
        {
            // userIdentifier will not work until Authentication and Authorization are configurated
            // use debugger to know exactly what next to configurate for JWT token.
            if (userIdentifier == null)
                _hubContext.Clients.All.ReceiveExport("Your file is ready.");
            else
                _hubContext.Clients.User(userIdentifier).ReceiveExport("Your file is ready.");
            return Ok();
        }
    }
}
