/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class EditModeView : YetaWFView, IYetaWFView<EditModeModule, EditModeModuleController.Model> {

        public const string ViewName = "EditMode";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(EditModeModule module, EditModeModuleController.Model model) {

            ModuleAction action = Manager.EditMode ? module.GetAction_SwitchToView() : module.GetAction_SwitchToEdit();
            string url = action.GetCompleteUrl(OnPage: true);

            if (Manager.EditMode) {

                // icon used: fas-pencil-alt
                return Task.FromResult($@"
<div id='yEditControlDiv'>
    <a class='t_edit y_button_outline y_button yaction-link' {Basics.CssTooltip}='{HAE(action.Tooltip)}' href='{HAE(url)}' rel='nofollow' data-button='' data-save-return=''>
        <svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 576 512'>
            <path fill='currentColor' d='M572.52 241.4C518.29 135.59 410.93 64 288 64S57.68 135.64 3.48 241.41a32.35 32.35 0 0 0 0 29.19C57.71 376.41 165.07 448 288 448s230.32-71.64 284.52-177.41a32.35 32.35 0 0 0 0-29.19zM288 400a144 144 0 1 1 144-144 143.93 143.93 0 0 1-144 144zm0-240a95.31 95.31 0 0 0-25.31 3.79 47.85 47.85 0 0 1-66.9 66.9A95.78 95.78 0 1 0 288 160z' class=''></path>
        </svg>
    </a>
</div>");
            } else {

                // icon used: fas-eye
                return Task.FromResult($@"
<div id='yEditControlDiv'>
    <a class='t_display y_button_outline y_button yaction-link' {Basics.CssTooltip}='{HAE(action.Tooltip)}' href='{HAE(url)}' rel='nofollow' data-button='' data-save-return=''>
        <svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 512 512'>
            <path fill='currentColor' d='M497.9 142.1l-46.1 46.1c-4.7 4.7-12.3 4.7-17 0l-111-111c-4.7-4.7-4.7-12.3 0-17l46.1-46.1c18.7-18.7 49.1-18.7 67.9 0l60.1 60.1c18.8 18.7 18.8 49.1 0 67.9zM284.2 99.8L21.6 362.4.4 483.9c-2.9 16.4 11.4 30.6 27.8 27.8l121.5-21.3 262.6-262.6c4.7-4.7 4.7-12.3 0-17l-111-111c-4.8-4.7-12.4-4.7-17.1 0zM124.1 339.9c-5.5-5.5-5.5-14.3 0-19.8l154-154c5.5-5.5 14.3-5.5 19.8 0s5.5 14.3 0 19.8l-154 154c-5.5 5.5-14.3 5.5-19.8 0zM88 424h48v36.3l-64.5 11.3-31.1-31.1L51.7 376H88v48z'></path>
        </svg>    
    </a>
</div>");
            }
        }
    }
}
