/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Controllers;
using YetaWF.Modules.Panels.Modules;

namespace YetaWF.Modules.Panels.Views {

    public class SplitterView : YetaWFView, IYetaWFView<SplitterModule, SplitterModuleController.Model> {

        public const string ViewName = "Splitter";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(SplitterModule module, SplitterModuleController.Model model) {
            return HtmlHelper.ForDisplayAsync(model, nameof(model.SplitterInfo));
        }
    }
}
