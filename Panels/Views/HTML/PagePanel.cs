using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Controllers;
using YetaWF.Modules.Panels.Modules;

namespace YetaWF.Modules.Panels.Views {

    public class PagePanelView : YetaWFView, IYetaWFView<PagePanelModule, PagePanelModuleController.ModelDisplay> {

        public const string ViewName = "PagePanel";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(PagePanelModule module, PagePanelModuleController.ModelDisplay model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.PanelInfo)));

            return hb.ToYHtmlString();
        }
    }
}
