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

        public async Task AddStandardAddOnsAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "jquery");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "jqueryui");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "necolas.github.io.normalize");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "no-margin-for-errors.com.prettyLoader");

            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_Core", "Basics");
            if (Manager.IsInPopup)
                await AddPopupsAddOnsAsync();
        }

        public async Task AddSkinAddOnsAsync() {

            SkinAccess skinAccess = new SkinAccess();

            // Find the jquery theme
            string skin = Manager.CurrentPage.jQueryUISkin;
            if (string.IsNullOrWhiteSpace(skin))
                skin = Manager.CurrentSite.jQueryUISkin;
            string jqueryUIFolder = await skinAccess.FindJQueryUISkinAsync(skin);
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "jqueryui-themes", jqueryUIFolder);

            // Find Kendo UI theme
            skin = Manager.CurrentPage.KendoUISkin;
            if (string.IsNullOrWhiteSpace(skin))
                skin = Manager.CurrentSite.KendoUISkin;
            string kendoUITheme = await skinAccess.FindKendoUISkinAsync(skin);
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "telerik.com.Kendo_UI_Core", kendoUITheme);
        }

        public async Task AddFormsAddOnsAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "bassistance.de.jquery-validation");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "microsoft.com.jquery_unobtrusive_validation");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "gist.github.com_remi_957732.jquery_validate_hooks");
        }

        public async Task AddPopupsAddOnsAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_Core", "Popups");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "Popups");
        }
    }
}