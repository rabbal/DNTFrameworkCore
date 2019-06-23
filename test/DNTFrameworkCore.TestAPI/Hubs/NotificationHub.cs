using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DNTFrameworkCore.TestAPI.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IUserSession _session;

        public NotificationHub(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public override async Task OnConnectedAsync()
        {
            //Todo: add Context.ConnectionId to Tenant group in MultiTenancy scenarios
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //Todo: remove Context.ConnectionId from Tenant group in MultiTenancy scenarios
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
        }
    }
}