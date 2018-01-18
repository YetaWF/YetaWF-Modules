/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feed#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Feed.Controllers;

namespace YetaWF.Modules.Feed.Addons {

    public class Info : IAddOnSupport {

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddVolatileOption(areaName, "DateFormat", UserSettings.GetProperty<Formatting.DateFormatEnum>("DateFormat"));

        }
    }
}