/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Views {

    public class GridRecordView : YetaWFView, IYetaWFView<ModuleDefinition, GridRecordData> {

        public const string ViewName = "GridRecord";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(ModuleDefinition module, GridRecordData model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(await HtmlHelper.ForDisplayContainerAsync(model, "GridRecord"));

            return hb.ToYHtmlString();
        }
    }
}
