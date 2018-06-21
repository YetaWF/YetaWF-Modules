/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons {

    public class Info : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            //ScriptManager scripts = manager.ScriptManager;
            //string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "something", Something);

            //scripts.AddLocalization(areaName, "something", this.__ResStrxxx("something", "something"));

            return Task.CompletedTask;
        }
    }
}
