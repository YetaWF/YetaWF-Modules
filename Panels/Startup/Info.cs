/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Panels.Controllers;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Addons {

    public class Info : IAddOnSupport {

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(areaName, "Action_Apply", PanelInfo.PanelAction.Apply);
            scripts.AddConfigOption(areaName, "Action_MoveLeft", PanelInfo.PanelAction.MoveLeft);
            scripts.AddConfigOption(areaName, "Action_MoveRight", PanelInfo.PanelAction.MoveRight);
            scripts.AddConfigOption(areaName, "Action_Add", PanelInfo.PanelAction.Add);
            scripts.AddConfigOption(areaName, "Action_Insert", PanelInfo.PanelAction.Insert);
            scripts.AddConfigOption(areaName, "Action_Remove", PanelInfo.PanelAction.Remove);

            scripts.AddLocalization(areaName, "RemoveConfirm", this.__ResStr("removeConfirm", "Are you sure you want to remove this panel?"));
            scripts.AddLocalization(areaName, "RemoveTitle", this.__ResStr("removeTitle", "Remove Panel"));
        }
    }
}