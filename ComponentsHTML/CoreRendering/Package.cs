/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        public Package GetImplementingPackage() {
            return AreaRegistration.CurrentPackage;
        }

        public async Task AddStandardAddOns() {
            await Manager.AddOnManager.AddAddOnGlobalAsync("jquery.com", "jquery");
            await Manager.AddOnManager.AddAddOnGlobalAsync("jqueryui.com", "jqueryui");
            await Manager.AddOnManager.AddAddOnGlobalAsync("medialize.github.io", "URI.js");// for client-side Url manipulation
            await Manager.AddOnManager.AddAddOnGlobalAsync("necolas.github.io", "normalize");
        }
    }
}
