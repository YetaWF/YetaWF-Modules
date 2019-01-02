/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons {

    public class Info : IAddOnSupport {

        //Sprites
        public static Dictionary<string, string> PredefSpriteIcons = new Dictionary<string, string> {
           { "#ModulePreview", "yic yic_componentshtml_modprev" },
           { "#PagePreview", "yic yic_componentshtml_pageprev" },
           { "#TextCopy", "yic yic_componentshtml_textcopy" },
           { "#TextAreaSourceOnlyCopy", "yic yic_componentshtml_textareasrccopy" },
           { "#UrlRemote", "yic yic_componentshtml_urlremote" },
           { "#ModuleMenuEdit", "yic yic_componentshtml_modmenuedit" },
        };

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            Package package = AreaRegistration.CurrentPackage;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddLocalization(areaName, "something", this.__ResStrxxx("something", "something"));

            scripts.AddConfigOption(areaName, "LoaderGif", manager.GetCDNUrl(manager.AddOnManager.GetAddOnNamedUrl(package.AreaName, "no-margin-for-errors.com.prettyLoader") + "images/prettyLoader/ajax-loader.gif"));

            return Task.CompletedTask;
        }
    }
}
