/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Controllers;

namespace YetaWF.Modules.Menus.Addons {

    public class Info : IAddOnSupport {

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddLocalization(areaName, "Separator", this.__ResStr("Separator", "(Separator)"));
            scripts.AddLocalization(areaName, "ChangedEntry", this.__ResStr("ChangedEntry", "You have modified the current menu entry - Please save it before selecting another menu entry"));
            scripts.AddLocalization(areaName, "NoMenuEntry", this.__ResStr("NoMenuEntry", "No menu entry selected"));
            scripts.AddLocalization(areaName, "NoResetMenu", this.__ResStr("NoResetMenu", "You can't reset the entire menu"));
            scripts.AddLocalization(areaName, "NoRemoveMenu", this.__ResStr("NoRemoveMenu", "You can't remove the entire menu"));

        }
    }
}