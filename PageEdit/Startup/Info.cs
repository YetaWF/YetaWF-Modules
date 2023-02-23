/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.PageEdit.DataProvider;

namespace YetaWF.Modules.PageEdit.Addons;

public class Info : IAddOnSupport {

    public const string PageControlMod = "yPageControlMod";

    public async Task AddSupportAsync(YetaWFManager manager) {

        ScriptManager scripts = manager.ScriptManager;
        string areaName = AreaRegistration.CurrentPackage.AreaName;
        ControlPanelConfigData config = await ControlPanelConfigDataProvider.GetConfigAsync();

        scripts.AddConfigOption(areaName, "PageControlMod", PageControlMod);
        scripts.AddConfigOption(areaName, "W3CUrl", config.W3CUrl);
    }
}