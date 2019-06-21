/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.DevTests.Controllers;
using YetaWF.Modules.DevTests.Modules;

namespace YetaWF.Modules.DevTests.Views {

    public class TemplateScrollerView : YetaWFView, IYetaWFView<TemplateScrollerModule, TemplateScrollerModuleController.Model> {

        public const string ViewName = "TemplateScroller";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(TemplateScrollerModule module, TemplateScrollerModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.Items)));

            return hb.ToString();
        }
    }
}
