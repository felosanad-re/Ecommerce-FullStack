using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Talabat.Core;

namespace Talabat.Repositaries.Data.Hubs
{
    public class HubSignalR : Hub
    {
        [Authorize] // Send Notifications For Users Who Make Order
        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"order-{orderId}" // Order owner Group -> order-10
                );
        }

        // Send Notification For All Admins
        [Authorize(Roles = SD.SUPER_ADMIN)]
        public async Task JoinAdminsGroup()
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                "Admins" // Group name 'Admins'
                );
        }
    }
}
