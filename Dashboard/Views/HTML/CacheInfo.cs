/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Dashboard.Controllers;
using YetaWF.Modules.Dashboard.Modules;

namespace YetaWF.Modules.Dashboard.Views {

    public class CacheInfoView : YetaWFView, IYetaWFView2<CacheInfoModule, CacheInfoModuleController.DisplayModel> {

        public const string ViewName = "CacheInfo";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(CacheInfoModule module, CacheInfoModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (Utility.AspNetMvc == Utility.AspNetMvcVersion.MVC5) {

                hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
            new FormButton() { ButtonType= ButtonTypeEnum.Cancel, Text=this.__ResStr("btnCancel", "Return") },
    })}
{await RenderEndFormAsync()}");

            } else {

                hb.Append($@"
    <div class='{Globals.CssDivWarning}'>
        {Utility.HE(this.__ResStr("notAvail", "Information not available on .NET 5.0"))}
    </div>");
            }
            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(CacheInfoModule module, CacheInfoModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForDisplayContainerAsync(model, "PropertyList"));
            return hb.ToString();

        }
    }
}
