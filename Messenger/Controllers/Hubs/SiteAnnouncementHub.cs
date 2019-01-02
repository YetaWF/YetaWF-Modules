/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

#if MVC6
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
#else
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using YetaWF.Core;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class YetaWF_Messenger_SiteAnnouncementsHub : Hub, ISignalRHub {

        public static Task SendMessageAsync(string text, string title) {
#if MVC6
            IHubContext<YetaWF_Messenger_SiteAnnouncementsHub> context = YetaWFManager.ServiceProvider.GetRequiredService<IHubContext<YetaWF_Messenger_SiteAnnouncementsHub>>();
            context.Clients.All.SendAsync("Message", text, title);
#else
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext(typeof(YetaWF_Messenger_SiteAnnouncementsHub).Name);
            context.Clients.All.Invoke("Message", text, title);
            return Task.CompletedTask;
#endif
        }
    }

}
