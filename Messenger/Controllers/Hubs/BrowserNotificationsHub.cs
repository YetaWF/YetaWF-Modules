/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Support;
using YetaWF.Core.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace YetaWF.Modules.Messenger.Controllers {

    public class YetaWF_Messenger_BrowserNotificationsHub : Hub, ISignalRHub {

        protected static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public void MapHub(IEndpointRouteBuilder bldr) {
            string url = SignalR.MakeUrl(nameof(YetaWF_Messenger_BrowserNotificationsHub));
            Logging.AddLog($"{nameof(YetaWF_Messenger_BrowserNotificationsHub)} adding route {url}");
            bldr.MapHub<YetaWF_Messenger_BrowserNotificationsHub>(url);
        }
        public override Task OnConnectedAsync() {
            return base.OnConnectedAsync();
        }

        public static Task SendMessageAsync(string title, string text, string icon, int timeout, string url) {
            IHubContext<YetaWF_Messenger_BrowserNotificationsHub> context = Manager.ServiceProvider.GetRequiredService<IHubContext<YetaWF_Messenger_BrowserNotificationsHub>>();
            return context.Clients.User(YetaWFManager.Manager.UserId.ToString()).SendAsync("Message", title, text, icon, timeout, url);
        }
    }
}
