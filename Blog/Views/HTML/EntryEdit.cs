using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class EntryEditView : YetaWFView, IYetaWFView2<EntryEditModule, EntryEditModuleController.EditModel> {

        public const string ViewName = "EntryEdit";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(EntryEditModule module, EntryEditModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text=this.__ResStr("btnApply", "Save") },
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, Text=this.__ResStr("btnSubmit", "Save & Display") },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            return hb.ToYHtmlString();
        }
        public async Task<YHtmlString> RenderPartialViewAsync(EntryEditModule module, EntryEditModuleController.EditModel model) {
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();
        }
    }
}
