/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.Messenger.Controllers;

namespace YetaWF.Modules.Messenger.Addons {

    public class Messaging : IAddOnSupport {

        public async Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            Package package = AreaRegistration.CurrentPackage;
            string area = package.AreaName;
            SkinImages skinImages = new SkinImages();

            scripts.AddLocalization(area, "msgOnlineTT", this.__ResStr("online", "User is online and can receive messages"));
            scripts.AddLocalization(area, "msgOfflineTT", this.__ResStr("offline", "User is offline and can't receive messages - If you send a message it will be stored and shown to the user at a later time"));
            scripts.AddLocalization(area, "msgNotSeenTT", this.__ResStr("notSeen", "This message has not been seen"));

            scripts.AddConfigOption(area, "msgOnlineIcon", await skinImages.FindIcon_PackageAsync("Online.png", package));
            scripts.AddConfigOption(area, "msgOfflineIcon", await skinImages.FindIcon_PackageAsync("Offline.png", package));
            scripts.AddConfigOption(area, "msgNotSeenIcon", await skinImages.FindIcon_PackageAsync("NotSeen.png", package));
        }
    }
}
