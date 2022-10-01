/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Controllers;
using YetaWF.Modules.Panels.Modules;

namespace YetaWF.Modules.Panels.Views {

    public class PageBarView : YetaWFView, IYetaWFView2<PageBarModule, PageBarModuleController.Model> {

        public const string ViewName = "PageBar";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(PageBarModule module, PageBarModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (Manager.EditMode) {

                hb.Append($@"
{await RenderBeginFormAsync(SaveReturnUrl: true)}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Apply, },
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            } else {

                hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.PanelInfo)));

            }
            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(PageBarModule module, PageBarModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToString();

        }
    }
}
