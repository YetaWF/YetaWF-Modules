/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.Controllers;

namespace YetaWF.Modules.Search.Addons {

    public class Info : IAddOnSupport {

        public const string UrlArg = "!YetaWF_Search_SearchControl";

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(areaName, "UrlArg", UrlArg);

        }
    }
}