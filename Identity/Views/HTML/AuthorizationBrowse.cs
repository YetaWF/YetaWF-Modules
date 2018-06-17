using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views {

    public class AuthorizationBrowseView : YetaWFView, IYetaWFView2<AuthorizationBrowseModule, AuthorizationBrowseModuleController.BrowseModel> {

        public const string ViewName = "AuthorizationBrowse";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(AuthorizationBrowseModule module, AuthorizationBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}");
            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(AuthorizationBrowseModule module, AuthorizationBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(this.__ResStr("explain", "<p>Resources are predefined by modules and packages. If a resource is deleted that is defined by an existing package, it is restored to its default state.</p>"));
            hb.Append(await HtmlHelper.ForDisplayAsync(model, "GridDef"));
            return hb.ToYHtmlString();

        }
    }
}
