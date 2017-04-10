/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/UserProfile#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.UserProfile.Controllers;

namespace YetaWF.Modules.UserProfile.Addons {

    public class Info : IAddOnSupport {

        public void AddSupport(YetaWFManager manager) {

            //ScriptManager scripts = manager.ScriptManager;
            //string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "something", Something);

            //scripts.AddLocalization(areaName, "something", this.__ResStr("something", "something"));
        }
    }
}
