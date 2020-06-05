using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DNTFrameworkCore.TestWebApp.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var groupName = Context.User.Identity.Name;
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var groupName = Context.User.Identity.Name;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await base.OnDisconnectedAsync(exception);
        }
    }
}