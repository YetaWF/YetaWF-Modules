/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Core;
using YetaWF.Modules.Messenger.Modules;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Messenger.Controllers {

    public class SkinSiteAnnouncementsModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.SkinSiteAnnouncementsModule> {

        public SkinSiteAnnouncementsModuleController() { }

        [AllowGet]
        public async Task<ActionResult> SkinSiteAnnouncements() {

            await SignalR.UseAsync();
            await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, nameof(SkinSiteAnnouncementsModule));
            Manager.ScriptManager.AddLast($"{AreaRegistration.CurrentPackage.AreaName}_{Module.ClassName}", $"new YetaWF_Messenger.SkinSiteAnnouncementsModule('{Utility.JserEncode(Module.ModuleHtmlId)}');");

            return new EmptyResult();
        }
    }
}
