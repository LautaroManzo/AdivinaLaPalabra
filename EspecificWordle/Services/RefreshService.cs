using EspecificWordle.Hubs;
using EspecificWordle.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace EspecificWordle.Services
{
    public class RefreshService : IRefreshService
    {
        private readonly IHubContext<RefreshHub> _hubContext;

        public RefreshService(IHubContext<RefreshHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void NotifyRefresh()
        {
            _hubContext.Clients.All.SendAsync("RefreshPage");
        }

    }
}
