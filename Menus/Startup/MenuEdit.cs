/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Controllers;

namespace YetaWF.Modules.Menus.Addons {

    public class MenuEdit : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddLocalization(areaName, "Separator", this.__ResStr("Separator", "(Separator)"));
            scripts.AddLocalization(areaName, "ChangedEntry", this.__ResStr("ChangedEntry", "You have modified the current menu entry - Please save it before selecting another menu entry"));
            scripts.AddLocalization(areaName, "NewEntry", this.__ResStr("NewEntry", "The current menu entry is a new entry - Please save it before selecting another menu entry"));
            scripts.AddLocalization(areaName, "NewEntryText", this.__ResStr("NewEntryText", "(New)"));

            return Task.CompletedTask;
        }
    }
}