/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
#else
using Microsoft.AspNet.SignalR;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class YetaWF_Messenger_BrowserNotificationsHub : Hub, ISignalRHub {

        public override Task OnConnected() {
            return base.OnConnected();
        }

        public static Task SendMessageAsync(string title, string text, string icon, int timeout, string url) {
#if MVC6
            $$$ IHubContext<YetaWF_Messenger_BrowserNotificationsHub> context = YetaWFManager.ServiceProvider.GetRequiredService<IHubContext<YetaWF_Messenger_BrowserNotificationsHub>>();
            $$$ context.Clients.User(YetaWFManager.Manager.UserName).SendAsync("Message", title, text, icon, timeout, url);
#else
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<YetaWF_Messenger_BrowserNotificationsHub>();
            context.Clients.User(YetaWFManager.Manager.UserName).Invoke("Message", title, text, icon, timeout, url);
            return Task.CompletedTask;
#endif
        }
    }

}
