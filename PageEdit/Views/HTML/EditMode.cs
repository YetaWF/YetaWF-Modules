/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class EditModeView : YetaWFView, IYetaWFView<EditModeModule, EditModeModuleController.Model> {

        public const string ViewName = "EditMode";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(EditModeModule module, EditModeModuleController.Model model) {

            // <div id='yEditControlDiv'>
            //  action button (with id yEditControlButton)
            // </div>

            ModuleAction action = Manager.EditMode ? module.GetAction_SwitchToView() : module.GetAction_SwitchToEdit();
            if (action != null)
                return $"<div id='yEditControlDiv'>{await action.RenderAsButtonIconAsync("yEditControlButton")}</div>";
            return null;
        }
    }
}
