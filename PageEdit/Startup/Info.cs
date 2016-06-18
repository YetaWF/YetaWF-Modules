/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEdit#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.PageEdit.Controllers;

namespace YetaWF.Modules.PageEdit.Addons {

    public class Info : IAddOnSupport {

        public const string PageControlMod = "yPageControlMod";

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(areaName, "PageControlMod", PageControlMod);
        }
    }
}