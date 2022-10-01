/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class EditModeView : YetaWFView, IYetaWFView<EditModeModule, EditModeModuleController.Model> {

        public const string ViewName = "EditMode";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(EditModeModule module, EditModeModuleController.Model model) {

            ModuleAction? action = Manager.EditMode ? module.GetAction_SwitchToView() : module.GetAction_SwitchToEdit();
            if (action != null) {
                string url = action.GetCompleteUrl(OnPage: true);

                if (Manager.EditMode) {

                    return Task.FromResult($@"
<div id='yEditControlDiv'>
    <a class='t_edit y_button_outline y_button yaction-link' {Basics.CssTooltip}='{HAE(action.Tooltip)}' href='{HAE(url)}' rel='nofollow' data-button='' data-save-return=''>
        {SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-pencil-alt")}
    </a>
</div>");
                } else {

                    return Task.FromResult($@"
<div id='yEditControlDiv'>
    <a class='t_display y_button_outline y_button yaction-link' {Basics.CssTooltip}='{HAE(action.Tooltip)}' href='{HAE(url)}' rel='nofollow' data-button='' data-save-return=''>
        {SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-eye")}
    </a>
</div>");
                }
            }
            return Task.FromResult(string.Empty);
        }
    }
}
