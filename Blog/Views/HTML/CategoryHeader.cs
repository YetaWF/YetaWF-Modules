/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class CategoryHeaderView : YetaWFView, IYetaWFView<CategoryHeaderModule, CategoryHeaderModule.DisplayModel> {

        public const string ViewName = "CategoryHeader";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(CategoryHeaderModule module, CategoryHeaderModule.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    <div class='t_desc'>
        {await HtmlHelper.ForDisplayAsync(model, nameof(model.Description))}
    </div>
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, Text=this.__ResStr("btnCancel", "Return") },
    })}
{await RenderEndFormAsync()}");

            return hb.ToString();
        }
    }
}
