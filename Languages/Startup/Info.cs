/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.Addons;

public class Info : IAddOnSupport {

    public Task AddSupportAsync(YetaWFManager manager) {
        ScriptManager scripts = manager.ScriptManager;
        string areaName = AreaRegistration.CurrentPackage.AreaName;

        scripts.AddLocalization(areaName, "ConfirmResetText", this.__ResStr("confirmResetText", "Are you sure you want to restore the default settings?"));
        return Task.CompletedTask;
    }
}
