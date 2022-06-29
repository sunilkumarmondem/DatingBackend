using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using DatingBackEnd.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DatingBackEnd.SignalIR
{
    [Authorize]
    public class PresenceHub : Hub
    {
       
       // private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            //_httpContextAccessor = new HttpContextAccessor();
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            //var userName = User?.Identity?.Name;
            //var isOnline = await _tracker.UserConnected(Context.User.Identity.Name, Context.ConnectionId);
            //if (isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.Identity.Name);

            //var currentUsers = await _tracker.GetOnlineUsers();
           // await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //var userName = User?.Identity?.Name;
            //var isOffline = await _tracker.UserDisconnected(Context.User.Identity.Name, Context.ConnectionId);

            //if (isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.Identity.Name);

            //await base.OnDisconnectedAsync(exception);
        }
    }
}
