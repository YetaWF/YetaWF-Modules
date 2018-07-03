/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons {

    public class Info : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            Package package = AreaRegistration.CurrentPackage;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddLocalization(areaName, "something", this.__ResStrxxx("something", "something"));

            scripts.AddConfigOption(areaName, "LoaderGif", manager.GetCDNUrl(manager.AddOnManager.GetAddOnNamedUrl(package.Domain, package.Product, "no-margin-for-errors.com.prettyLoader") + "images/prettyLoader/ajax-loader.gif"));

            return Task.CompletedTask;
        }
    }
}
