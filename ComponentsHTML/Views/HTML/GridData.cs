using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Views {

    public class GridDataView : YetaWFView, IYetaWFView<ModuleDefinition, DataSourceResult> {

        public const string ViewName = "GridData";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(ModuleDefinition module, DataSourceResult model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(await HtmlHelper.ForDisplayContainerAsync(model, "GridData"));

            return hb.ToYHtmlString();
        }
    }
}
