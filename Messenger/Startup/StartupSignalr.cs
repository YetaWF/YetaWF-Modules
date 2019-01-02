/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using YetaWF.Core.Support;
using YetaWF.Core;
#if MVC6
#else
using Microsoft.AspNet.SignalR;
using Owin;
#endif

namespace YetaWF.Modules.Messenger {

#if MVC6
#else
    public class SignalRInit : IInitializeOwinStartup {

        public void InitializeOwinStartup(Owin.IAppBuilder app) {
            HubConfiguration hubConfig = new HubConfiguration();
#if DEBUG
            hubConfig.EnableDetailedErrors = true;
#endif
            hubConfig.EnableJavaScriptProxies = false;
            app.MapSignalR(SignalR.SignalRUrl, hubConfig);
        }
    }
#endif
}
