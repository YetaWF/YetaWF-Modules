/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.DevTests.Modules;

namespace YetaWF.Modules.DevTests.Views {

    public class TestEscapesView : YetaWFView, IYetaWFView2<TestEscapesModule, TestEscapesModule.EditModel> {

        public const string ViewName = "TestEscapes";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(TestEscapesModule module, TestEscapesModule.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync(HtmlAttributes: new { id = DivId })}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    <div class='t_tests'>
        <input type='button' class='y_button' value='message()' name='message'>
        <input type='button' class='y_button y_warning' value='warning()' name='warning'>
        <input type='button' class='y_button y_danger' value='error()' name='error'>
        <input type='button' class='y_button' value='alertYesNo()' name='alertYesNo'>
        <input type='button' class='y_button y_success' value='confirm()' name='confirm'>
        <input type='button' class='y_button' value='pleaseWait()' name='pleaseWait'>");

#if DEBUG
            hb.Append($@"
        <input type='button' class='y_button' value='JavaScript Error' name='jserror'>");
#endif

            hb.Append($@"
    </div>
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text= "Apply (Test <A> &amp; & @ {0})", Title = "Tooltip with special characters <A> &amp; & @ {0}" },
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");
            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(TestEscapesModule module, TestEscapesModule.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToString();

        }
    }
}
