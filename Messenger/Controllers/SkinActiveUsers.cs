/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Core;
using YetaWF.Modules.Messenger.Modules;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Messenger.Controllers {

    public class SkinActiveUsersModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.SkinActiveUsersModule> {

        public SkinActiveUsersModuleController() { }

        [AllowGet]
        public async Task<ActionResult> SkinActiveUsers() {

            await SignalR.UseAsync();
            await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, nameof(SkinActiveUsersModule));
            Manager.ScriptManager.AddLast($"{AreaRegistration.CurrentPackage.AreaName}_{Module.ClassName}", $"new YetaWF_Messenger.SkinActiveUsersModule('{Utility.JserEncode(Module.ModuleHtmlId)}');");

            return new EmptyResult();
        }
    }
}
