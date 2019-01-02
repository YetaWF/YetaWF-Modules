/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class EditModeView : YetaWFView, IYetaWFView<EditModeModule, EditModeModuleController.Model> {

        public const string ViewName = "EditMode";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(EditModeModule module, EditModeModuleController.Model model) {

            // this view is used to include js/css only
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append("<!-- A comment so we generate something, otherwise js/css is not included -->");
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
