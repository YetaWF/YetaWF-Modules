﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class SkinSelectionView : YetaWFView, IYetaWFView2<PageControlModule, PageControlModuleController.SkinSelectionModel> {

        public const string ViewName = "SkinSelection";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(PageControlModule module, PageControlModuleController.SkinSelectionModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => await RenderPartialViewAsync(module, model), UsePartialFormCss: false)}
            <div class='t_detailsbuttons yNoPrint'>
                <input type='submit' value='{this.__ResStr("saveSkins", "Save & Display")}' />
            </div>
        {await RenderEndFormAsync()}");

            return hb.ToYHtmlString();
        }
        public async Task<YHtmlString> RenderPartialViewAsync(PageControlModule module, PageControlModuleController.SkinSelectionModel model) {
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();
        }
    }
}
