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

    <input type='button' value='message()' name='message' />
    <input type='button' value='error()' name='error' />
    <input type='button' value='alert()' name='alert' />
    <input type='button' value='alertYesNo()' name='alertYesNo' />
    <input type='button' value='confirm()' name='confirm' />
    <input type='button' value='pleaseWait()' name='pleaseWait' />

    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text= "Apply (Test <A> &amp; & @ {0})", Title = "Tooltip with special characters <A> &amp; & @ {0}" },
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}

<script>
    $('#{DivId}').on('click', 'input[name=""message""]', function () {{
        $YetaWF.message('TEST <A> &amp; & @ {0} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""error""]', function () {{
        $YetaWF.error('TEST <A> &amp; & @ {{0}} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""alert""]', function () {{
        $YetaWF.alert('TEST <A> &amp; & @ {{0}} TEST', 'TITLE <A> &amp; & @ {{0}} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""confirm""]', function () {{
        $YetaWF.confirm('TEST <A> &amp; & @ {{0}} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""alertYesNo""]', function () {{
        $YetaWF.alertYesNo('TEST <A> &amp; & @ {{0}} TEST', 'TITLE <A> &amp; & @ {{0}} TEST');
    }});
    $('#{DivId}').on('click', 'input[name=""pleaseWait""]', function () {{
        $YetaWF.pleaseWait('Reload page to continue\n\nTEST <A> &amp; & @ {{0}} TEST', 'TITLE <A> &amp; & @ {{0}} TEST');
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
