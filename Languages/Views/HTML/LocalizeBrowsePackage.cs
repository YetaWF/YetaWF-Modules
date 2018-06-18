using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Languages.Controllers;
using YetaWF.Modules.Languages.Modules;

namespace YetaWF.Modules.Languages.Views {

    public class LocalizeBrowsePackageView : YetaWFView, IYetaWFView2<LocalizeBrowsePackageModule, LocalizeBrowsePackageModuleController.BrowseModel> {

        public const string ViewName = "LocalizeBrowsePackage";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(LocalizeBrowsePackageModule module, LocalizeBrowsePackageModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (!LocalizationSupport.UseLocalizationResources) {
                hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {HE(this.__ResStr("disabled1", "Localization support is currently disabled - Use Localization Settings to enable."))}
</div>");
            } else if (!Manager.CurrentSite.Localization) {
                hb.Append($@"
<div class='{HE(Globals.CssDivWarning)}'>
    {HE(this.__ResStr("disabled2", "Localization support is currently disabled - Use Site Settings to enable."))}
</div>");
            }

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await HtmlHelper.ForDisplayAsync(model, "GridDef")}
{await RenderEndFormAsync()}");

            return hb.ToYHtmlString();
        }

        public Task<YHtmlString> RenderPartialViewAsync(LocalizeBrowsePackageModule module, LocalizeBrowsePackageModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            return Task.FromResult(hb.ToYHtmlString());

        }
    }
}
