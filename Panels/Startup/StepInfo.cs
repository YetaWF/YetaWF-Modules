/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Panels.Addons.Templates {

    public class StepInfoEdit : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddLocalization(areaName, "RemoveStepConfirm", this.__ResStr("removeStepConfirm", "Are you sure you want to remove this step?"));
            scripts.AddLocalization(areaName, "RemoveStepTitle", this.__ResStr("removeStepTitle", "Remove Step"));

            return Task.CompletedTask;
        }
    }
}