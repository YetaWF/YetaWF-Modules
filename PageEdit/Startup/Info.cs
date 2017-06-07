/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.DataProvider;

namespace YetaWF.Modules.PageEdit.Addons {

    public class Info : IAddOnSupport {

        public const string PageControlMod = "yPageControlMod";

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;
            ControlPanelConfigData config = ControlPanelConfigDataProvider.GetConfig();

            scripts.AddConfigOption(areaName, "PageControlMod", PageControlMod);
            scripts.AddConfigOption(areaName, "W3CUrl", config.W3CUrl);
        }
    }
}