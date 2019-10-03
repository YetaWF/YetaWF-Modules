/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Support;
using YetaWF.Core.Log;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
#if MVC6
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
#else
using Microsoft.AspNet.SignalR;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class YetaWF_Messenger_BrowserNotificationsHub : Hub, ISignalRHub {

#if MVC6
        public void MapHub(IEndpointRouteBuilder bldr) {
            string url = SignalR.MakeUrl(nameof(YetaWF_Messenger_BrowserNotificationsHub));
            Logging.AddLog($"{nameof(YetaWF_Messenger_BrowserNotificationsHub)} adding route {url}");
            bldr.MapHub<YetaWF_Messenger_BrowserNotificationsHub>(url);
        }
        public override Task OnConnectedAsync() {
            return base.OnConnectedAsync();
        }
#else
        public override Task OnConnected() {
            return base.OnConnected();
        }
#endif

        public static Task SendMessageAsync(string title, string text, string icon, int timeout, string url) {
#if MVC6
            IHubContext<YetaWF_Messenger_BrowserNotificationsHub> context = YetaWFManager.ServiceProvider.GetRequiredService<IHubContext<YetaWF_Messenger_BrowserNotificationsHub>>();
            return context.Clients.User(YetaWFManager.Manager.UserId.ToString()).SendAsync("Message", title, text, icon, timeout, url);
#else
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<YetaWF_Messenger_BrowserNotificationsHub>();
            context.Clients.User(YetaWFManager.Manager.UserName).Invoke("Message", title, text, icon, timeout, url);
            return Task.CompletedTask;
#endif
        }
    }
}
