/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        public Package GetImplementingPackage() {
            return AreaRegistration.CurrentPackage;
        }

        public async Task AddStandardAddOns() {
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "jquery");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "jqueryui");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "medialize.github.io.URI.js");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "necolas.github.io.normalize");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "no-margin-for-errors.com.prettyLoader");
        }

        public async Task AddSkinAddOns() {

            SkinAccess skinAccess = new SkinAccess();

            // Find the jquery theme
            string skin = Manager.CurrentPage.jQueryUISkin;
            if (string.IsNullOrWhiteSpace(skin))
                skin = Manager.CurrentSite.jQueryUISkin;
            string jqueryUIFolder = await skinAccess.FindJQueryUISkinAsync(skin);
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "jqueryui-themes", jqueryUIFolder);

            // Find Kendo UI theme
            skin = Manager.CurrentPage.KendoUISkin;
            if (string.IsNullOrWhiteSpace(skin))
                skin = Manager.CurrentSite.KendoUISkin;
            string kendoUITheme = await skinAccess.FindKendoUISkinAsync(skin);
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "telerik.com.Kendo_UI_Core", kendoUITheme);
        }

        public async Task AddFormsAddOns() {
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "bassistance.de.jquery-validation");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "microsoft.com.jquery_unobtrusive_validation");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "gist.github.com_remi_957732.jquery_validate_hooks");
        }
    }
}