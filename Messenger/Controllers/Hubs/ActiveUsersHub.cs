/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Log;
using YetaWF.Modules.Messenger.DataProvider;

namespace YetaWF.Modules.Messenger.Controllers {

    public class YetaWF_Messenger_ActiveUsersHub : Hub, ISignalRHub {

        public void MapHub(IEndpointRouteBuilder bldr) {
            string url = SignalR.MakeUrl(nameof(YetaWF_Messenger_ActiveUsersHub));
            Logging.AddLog($"{nameof(YetaWF_Messenger_ActiveUsersHub)} adding route {url}");
            bldr.MapHub<YetaWF_Messenger_ActiveUsersHub>(url);
        }

        public override async Task OnConnectedAsync() {
            int userId = 0;
            try {
                userId = Convert.ToInt32(Context.UserIdentifier);
            } catch (Exception) {
            }
            await SignalR.SetupSignalRHubAsync(this, async () => {
                using (ActiveUsersDataProvider usersDP = new ActiveUsersDataProvider()) {
                    await usersDP.RemoveConnectionAsync(Context.ConnectionId);
                    await usersDP.AddConnectionAsync(Context.ConnectionId, userId);
                }
            });
        }
        public override async Task OnDisconnectedAsync(Exception? exception) {

            await SignalR.SetupSignalRHubAsync(this, async () => {
                using (ActiveUsersDataProvider usersDP = new ActiveUsersDataProvider()) {
                    await usersDP.RemoveConnectionAsync(Context.ConnectionId);
                }
            });

            await base.OnDisconnectedAsync(exception);
        }
    }
}
