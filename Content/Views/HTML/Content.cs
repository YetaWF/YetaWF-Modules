/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Content.Controllers;
using YetaWF.Modules.Content.Modules;

namespace YetaWF.Modules.Content.Views {

    public class TextView : YetaWFView, IYetaWFView2<ContentModule, ContentModuleController.ContentModel> {

        public const string ViewName = "Content";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(ContentModule module, ContentModuleController.ContentModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            List<FormButton> buttons = new List<FormButton> {
                new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text=this.__ResStr("btnApply", "Save") },
            };
            if (Manager.IsInPopup)
                buttons.Add(new FormButton() { ButtonType = ButtonTypeEnum.Cancel, });
            else
                buttons.Add(new FormButton() { ButtonType = ButtonTypeEnum.Submit, Text = this.__ResStr("btnSave", "Save & Display"), Title = this.__ResStr("btnSaveTT", "Save all text and display the page in Site View mode") });

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model), UsePartialFormCss: false)}
    {await HtmlHelper.ForEditAsync(model, nameof(model.Contents))}
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.Url))}
    {await FormButtonsAsync(buttons)}
{await RenderEndFormAsync()}");
            return hb.ToYHtmlString();
        }

        public Task<YHtmlString> RenderPartialViewAsync(ContentModule module, ContentModuleController.ContentModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            return Task.FromResult(hb.ToYHtmlString());

        }
    }
}
