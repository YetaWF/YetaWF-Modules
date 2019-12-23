/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using YetaWF.Core.Log;
#else
using Microsoft.AspNet.SignalR;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class YetaWF_Messenger_SiteAnnouncementsHub : Hub, ISignalRHub {

#if MVC6
        public void MapHub(IEndpointRouteBuilder bldr) {
            string url = SignalR.MakeUrl(nameof(YetaWF_Messenger_SiteAnnouncementsHub));
            Logging.AddLog($"{nameof(YetaWF_Messenger_SiteAnnouncementsHub)} adding route {url}");
            bldr.MapHub<YetaWF_Messenger_SiteAnnouncementsHub>(url);
        }
#else
#endif

        public static Task SendMessageAsync(string text, string title) {
#if MVC6
            IHubContext<YetaWF_Messenger_SiteAnnouncementsHub> context = YetaWFManager.ServiceProvider.GetRequiredService<IHubContext<YetaWF_Messenger_SiteAnnouncementsHub>>();
            return context.Clients.All.SendAsync("Message", text, title);
#else
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext(typeof(YetaWF_Messenger_SiteAnnouncementsHub).Name);
            context.Clients.All.Invoke("Message", text, title);
            return Task.CompletedTask;
#endif
        }
    }

}
