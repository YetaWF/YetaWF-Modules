/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views;

public class CommentAddView : YetaWFView, IYetaWFView2<CommentAddModule, CommentAddModule.AddModel> {

    public const string ViewName = "CommentAdd";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(CommentAddModule module, CommentAddModule.AddModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        if (model.OpenForComments) {

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
            new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
            new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

        } else {

            hb.Append($@"
<div class='t_closedforcomments'>
    {Utility.HE(this.__ResStr("closed", "Closed for comments"))}
</div>");
        }

        return hb.ToString();
    }
    public async Task<string> RenderPartialViewAsync(CommentAddModule module, CommentAddModule.AddModel model) {
        HtmlBuilder hb = new HtmlBuilder();
        hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
        return hb.ToString();
    }
}
