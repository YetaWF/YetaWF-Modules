/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Panels.Controllers;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Addons {

    public class Info : IAddOnSupport {

        public const string Resource_AllowListOfLocalPagesAjax = "YetaWF_Panels-AllowListOfLocalPagesAjax";

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(areaName, "Action_Apply", PanelInfo.PanelAction.Apply);
            scripts.AddConfigOption(areaName, "Action_MoveLeft", PanelInfo.PanelAction.MoveLeft);
            scripts.AddConfigOption(areaName, "Action_MoveRight", PanelInfo.PanelAction.MoveRight);
            scripts.AddConfigOption(areaName, "Action_Add", PanelInfo.PanelAction.Add);
            scripts.AddConfigOption(areaName, "Action_Insert", PanelInfo.PanelAction.Insert);
            scripts.AddConfigOption(areaName, "Action_Remove", PanelInfo.PanelAction.Remove);

            scripts.AddLocalization(areaName, "RemovePanelConfirm", this.__ResStr("removePanelConfirm", "Are you sure you want to remove this panel?"));
            scripts.AddLocalization(areaName, "RemovePanelTitle", this.__ResStr("removePanelTitle", "Remove Panel"));

            scripts.AddLocalization(areaName, "RemoveStepConfirm", this.__ResStr("removeStepConfirm", "Are you sure you want to remove this step?"));
            scripts.AddLocalization(areaName, "RemoveStepTitle", this.__ResStr("removeStepTitle", "Remove Step"));

            return Task.CompletedTask;
        }
    }
}