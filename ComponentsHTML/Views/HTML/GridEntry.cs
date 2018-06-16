using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Views {

    public class GridEntryView : YetaWFView, IYetaWFView<ModuleDefinition, GridDefinition.GridEntryDefinition> {

        public const string ViewName = "GridEntry";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(ModuleDefinition module, GridDefinition.GridEntryDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            using (Manager.StartNestedComponent(string.Format("{0}[{1}]", model.Prefix, model.RecNumber))) {
                hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.Model)));
            }

            return hb.ToYHtmlString();
        }
    }
}
