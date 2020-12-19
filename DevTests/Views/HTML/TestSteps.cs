/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.DevTests.Controllers;
using YetaWF.Modules.DevTests.Modules;

namespace YetaWF.Modules.DevTests.Views {

    public class TestStepsView : YetaWFView, IYetaWFView<TestStepsModule, TestStepsModuleController.Model> {

        public const string ViewName = "TestSteps";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(TestStepsModule module, TestStepsModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await HtmlHelper.ForDisplayContainerAsync(model, "PropertyList")}

<input type='button' value='clear()' name='Clear' />
<input type='button' value='activateAll()' name='ActivateAll' />
<input type='button' value='activateLast()' name='ActivateLast' />
<br/>
<br/>
<input type='button' value='step1' name='Step1' />
<input type='button' value='step2' name='Step2' />
<input type='button' value='step3' name='Step3' />
<input type='button' value='step4' name='Step4' />
<br/>
<br/>
<input type='button' value='step1' name='Step1' />
<input type='button' value='step1-step2' name='Step12' />
<input type='button' value='step1-step3' name='Step13' />
<input type='button' value='step1-step4' name='Step14' />");

            Manager.ScriptManager.AddLast($@"
if (typeof YetaWF_Panels !== 'undefined' && YetaWF_Panels.StepInfoComponent) {{
    YetaWF_Panels.StepInfoComponent.setActive({{Name: 'YetaWF_DevTests_Step1'}});
}}");

            return hb.ToString();
        }
    }
}
