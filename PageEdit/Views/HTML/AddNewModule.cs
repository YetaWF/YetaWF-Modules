/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class AddNewModuleView : YetaWFView, IYetaWFView2<PageControlModule, PageControlModuleController.AddNewModuleModel> {

        public const string ViewName = "AddNewModule";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(PageControlModule module, PageControlModuleController.AddNewModuleModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => await RenderPartialViewAsync(module, model), UsePartialFormCss: false)}
            <div class='t_detailsbuttons yNoPrint'>
                <input type='submit' value='{this.__ResStr("addNewModule", "Add")}' />
            </div>
        {await RenderEndFormAsync()}");

            return hb.ToString();
        }
        public async Task<string> RenderPartialViewAsync(PageControlModule module, PageControlModuleController.AddNewModuleModel model) {
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToString();
        }
    }
}
