using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Text.Controllers;
using YetaWF.Modules.Text.Modules;

namespace YetaWF.Modules.Text.Views {

    public class TextDisplayView : YetaWFView, IYetaWFView<TextModule, TextModuleController.TextModelDisplay> {

        public const string ViewName = "TextDisplay";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(TextModule module, TextModuleController.TextModelDisplay model) {

            HtmlBuilder hb = new HtmlBuilder();

            List<FormButton> buttons = new List<FormButton> {
                new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text=this.__ResStr("btnApply", "Save") },
            };
            if (Manager.IsInPopup)
                buttons.Add(new FormButton() { ButtonType = ButtonTypeEnum.Cancel, });
            else
                buttons.Add(new FormButton() { ButtonType = ButtonTypeEnum.Submit, Text = this.__ResStr("btnSave", "Save & Display"), Title = this.__ResStr("btnSaveTT", "Save all text and display the page in Site View mode") });

            hb.Append($@"
{await HtmlHelper.ForDisplayAsync(model, nameof(model.Contents))}");
            return hb.ToYHtmlString();
        }
    }
}
