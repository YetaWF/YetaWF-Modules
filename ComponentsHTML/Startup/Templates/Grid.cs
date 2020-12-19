/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    /// <summary>
    /// Implements the YetaWF.Core.Addons.IAddOnSupport interface, which is called when the framework detects the use of the Grid component type.
    /// The AddSupportAsync method is called so Grid component specific configuration options and localizations can be added to the page.
    /// </summary>
    public class Grid : IAddOnSupport {

        /// <summary>
        /// Called by the framework so the component can add component specific client-side configuration options and localizations to the page.
        /// </summary>
        /// <param name="manager">The YetaWF.Core.Support.Manager instance of current HTTP request.</param>
        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "something", Something);

            scripts.AddLocalization(areaName, "GridTotalNone", this.__ResStr("gridTotalNone", "No items"));
            scripts.AddLocalization(areaName, "GridTotal0", this.__ResStr("gridTotal0", "None of {0} items"));
            scripts.AddLocalization(areaName, "GridTotals", this.__ResStr("gridTotals", "{0} - {1} of {2} items"));

            return Task.CompletedTask;
        }
    }
}
