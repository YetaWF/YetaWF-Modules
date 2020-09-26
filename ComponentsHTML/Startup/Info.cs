/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons {

    /// <summary>
    /// Implements the YetaWF.Core.Addons.IAddOnSupport interface, which is called when the framework detects the use of the YetaWF.ComponentsHTML package.
    /// The AddSupportAsync method is called so package specific configuration options and localizations can be added to the page.
    /// </summary>
    public class Info : IAddOnSupport {

        /// <summary>
        /// Sprites used by the package.
        /// </summary>
        public static Dictionary<string, string> PredefSpriteIcons = new Dictionary<string, string> {
           { "#ModulePreview", "yic yic_componentshtml_modprev" },
           { "#PagePreview", "yic yic_componentshtml_pageprev" },
           { "#TextCopy", "yic yic_componentshtml_textcopy" },
           { "#TextAreaSourceOnlyCopy", "yic yic_componentshtml_textareasrccopy" },
           { "#UrlRemote", "yic yic_componentshtml_urlremote" },
           { "#ModuleMenuEdit", "yic yic_componentshtml_modmenuedit" },
        };

        /// <summary>
        /// Called by the framework so the package can add package specific client-side configuration options and localizations to the page.
        /// </summary>
        /// <param name="manager">The YetaWF.Core.Support.Manager instance of current HTTP request.</param>
        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            Package package = AreaRegistration.CurrentPackage;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(areaName, "LoaderGif", manager.GetCDNUrl(VersionManager.GetAddOnPackageUrl(package.AreaName) + "images/ajax-loader.gif"));

            return Task.CompletedTask;
        }
    }
}
