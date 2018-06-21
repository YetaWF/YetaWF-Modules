/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.DevTests.Controllers;
using YetaWF.Modules.DevTests.Modules;

namespace YetaWF.Modules.DevTests.Views {

    public class TestEscapesView : YetaWFView, IYetaWFView2<TestEscapesModule, TestEscapesModuleController.EditModel> {

        public const string ViewName = "TestEscapes";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(TestEscapesModule module, TestEscapesModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync(HtmlAttributes: new { id = DivId })}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}

    <input type='button' value='Y_Message()' name='Y_Message' />
    <input type='button' value='Y_Error()' name='Y_Error' />
    <input type='button' value='Y_Alert()' name='Y_Alert' />
    <input type='button' value='Y_AlertYesNo()' name='Y_AlertYesNo' />
    <input type='button' value='Y_Confirm()' name='Y_Confirm' />
    <input type='button' value='Y_PleaseWait()' name='Y_PleaseWait' />

    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text= "Apply (Test <A> &amp; & @ {0})", Title = "Tooltip with special characters <A> &amp; & @ {0}" },
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}

<script>
    $('#{DivId}').on('click', 'input[name=""Y_Message""]', function () {{
        Y_Message('TEST <A> &amp; & @ {0} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""Y_Error""]', function () {{
        Y_Error('TEST <A> &amp; & @ {{0}} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""Y_Alert""]', function () {{
        Y_Alert('TEST <A> &amp; & @ {{0}} TEST', 'TITLE <A> &amp; & @ {{0}} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""Y_Confirm""]', function () {{
        Y_Confirm('TEST <A> &amp; & @ {{0}} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""Y_AlertYesNo""]', function () {{
        Y_AlertYesNo('TEST <A> &amp; & @ {{0}} TEST', 'TITLE <A> &amp; & @ {{0}} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""Y_PleaseWait""]', function () {{
        Y_PleaseWait('Reload page to continue\n\nTEST <A> &amp; & @ {{0}} TEST', 'TITLE <A> &amp; & @ {{0}} TEST');
    }});
</script>
");

            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(TestEscapesModule module, TestEscapesModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();

        }
    }
}
