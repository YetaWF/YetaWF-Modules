/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        internal class ComponentsData {
            public bool KendoUIUsed { get; set; }
            public bool JqueryUIUsed { get; set; }
        }

        internal static ComponentsData GetComponentsData() {
            ComponentsData cData = (ComponentsData)Manager.ComponentsData;
            if (cData == null) {
                cData = new ComponentsData();
                Manager.ComponentsData = cData;
            }
            return cData;
        }

        /// <summary>
        /// Returns the package that implements the YetaWF.Core.Components.IYetaWFCoreRendering interface.
        /// </summary>
        /// <returns>Returns the package that implements the YetaWF.Core.Components.IYetaWFCoreRendering interface.</returns>
        public Package GetImplementingPackage() {
            return AreaRegistration.CurrentPackage;
        }
        /// <summary>
        /// Adds any addons that are required by the package rendering components and views.
        /// </summary>
        public async Task AddStandardAddOnsAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "necolas.github.io.normalize");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "jquery");

            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_Core", "Basics");
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_Core", "Icons");

            if (Manager.IsInPopup)
                await AddPopupsAddOnsAsync();

            await Manager.AddOnManager.AddPackageAsync(Package, new List<Package>());
        }
        /// <summary>
        /// Adds any skin-specific addons for the current page that are required by the package rendering components and views.
        /// </summary>
        public async Task AddSkinAddOnsAsync() {
            SkinAccess skinAccess = new SkinAccess();
            string skin;

            // add jqueryui theme folder in case we need to dynamically load jqueryui from the client
            // Find the jquery theme+
            skin = Manager.CurrentPage.jQueryUISkin;
            if (string.IsNullOrWhiteSpace(skin))
                skin = Manager.CurrentSite.jQueryUISkin;
            string jqueryUIFolder = await skinAccess.FindJQueryUISkinAsync(skin);
            Manager.ScriptManager.AddVolatileOption(AreaRegistration.CurrentPackage.AreaName, "jqueryUITheme", jqueryUIFolder);

            // add kendoui theme folder in case we need to dynamically load kendoui from the client
            // Find the kendo theme
            skinAccess = new SkinAccess();
            skin = Manager.CurrentPage.KendoUISkin;
            if (string.IsNullOrWhiteSpace(skin))
                skin = Manager.CurrentSite.KendoUISkin;
            string kendoUITheme = await skinAccess.FindKendoUISkinAsync(skin);
            Manager.ScriptManager.AddVolatileOption(AreaRegistration.CurrentPackage.AreaName, "kendoUITheme", kendoUITheme);
        }

        /// <summary>
        /// Adds any form-specific addons for the current page that are required by the package rendering components and views.
        /// </summary>
        /// <remarks>This is only called if a page contains a form.</remarks>
        public async Task AddFormsAddOnsAsync() {
            SkinAccess skinAccess = new SkinAccess();
            if (!Manager.SkinInfo.UsingBootstrap || !Manager.SkinInfo.UsingBootstrapButtons)
                await JqueryUICore.UseAsync(); // using jquery UI buttons
        }
        /// <summary>
        /// Adds any popup-specific addons for the current page that are required by the package rendering components and views.
        /// </summary>
        /// <remarks>This is only called if a page can contain a popup.</remarks>
        public async Task AddPopupsAddOnsAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_Core", "Popups");
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "Popups");
            await JqueryUICore.UseAsync();//Css for popups
        }
    }
}