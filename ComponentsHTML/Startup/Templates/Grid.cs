/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class Grid : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "something", Something);

            scripts.AddLocalization(areaName, "GridTotalNone", this.__ResStr("gridTotals", "No items"));
            scripts.AddLocalization(areaName, "GridTotal0", this.__ResStr("gridTotals", "None of {0} items"));
            scripts.AddLocalization(areaName, "GridTotals", this.__ResStr("gridTotals", "{0} - {1} of {2} items"));

            return Task.CompletedTask;
        }
    }
}
