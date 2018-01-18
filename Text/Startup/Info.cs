/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Text.Controllers;

namespace YetaWF.Modules.Text.Addons {

    public class Info : IAddOnSupport {

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

        }
    }
}