/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Support;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using YetaWF.Core.Log;

namespace YetaWF.Modules.Messenger.Controllers {

    public class YetaWF_Messenger_SiteAnnouncementsHub : Hub, ISignalRHub {

        protected static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public void MapHub(IEndpointRouteBuilder bldr) {
            string url = SignalR.MakeUrl(nameof(YetaWF_Messenger_SiteAnnouncementsHub));
            Logging.AddLog($"{nameof(YetaWF_Messenger_SiteAnnouncementsHub)} adding route {url}");
            bldr.MapHub<YetaWF_Messenger_SiteAnnouncementsHub>(url);
        }

        public static Task SendMessageAsync(string text, string title) {
            IHubContext<YetaWF_Messenger_SiteAnnouncementsHub> context = Manager.ServiceProvider.GetRequiredService<IHubContext<YetaWF_Messenger_SiteAnnouncementsHub>>();
            return context.Clients.All.SendAsync("Message", text, title);
        }
    }

}
