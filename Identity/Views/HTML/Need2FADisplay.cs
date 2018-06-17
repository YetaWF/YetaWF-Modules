using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views {

    public class Need2FADisplayView : YetaWFView, IYetaWFView<Need2FADisplayModule, Need2FADisplayModuleController.DisplayModel> {

        public const string ViewName = "Need2FADisplay";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(Need2FADisplayModule module, Need2FADisplayModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "Need2FADisplay");

            hb.Append($@"
<div class='t_message'>
    {HE(this.__ResStr("need2FA", "Please set up Two-Step Authentication for full access to this site - "))}
    {await model.SetupAction.RenderAsLinkAsync()}
</div>
<script>
 $('#{module.ModuleHtmlId}').prependTo('body');
</script>
");

            return hb.ToYHtmlString();
        }
    }
}
