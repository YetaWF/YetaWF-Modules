/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Panels.Controllers;

namespace YetaWF.Modules.Panels.Addons.Templates {

    public class PanelInfo : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddLocalization(areaName, "RemovePanelConfirm", this.__ResStr("removePanelConfirm", "Are you sure you want to remove this panel?"));
            scripts.AddLocalization(areaName, "RemovePanelTitle", this.__ResStr("removePanelTitle", "Remove Panel"));

            return Task.CompletedTask;
        }
    }
}