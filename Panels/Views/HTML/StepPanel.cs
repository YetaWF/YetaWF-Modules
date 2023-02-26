/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Modules;

namespace YetaWF.Modules.Panels.Views;

public class DisplayStepsView : YetaWFView, IYetaWFView2<DisplayStepsModule, DisplayStepsModule.Model> {

    public const string ViewName = "DisplaySteps";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(DisplayStepsModule module, DisplayStepsModule.Model model) {

        HtmlBuilder hb = new HtmlBuilder();

        if (Manager.EditMode) {

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}");

        } else {

            hb.Append($@"
{await HtmlHelper.ForDisplayAsync(model, nameof(model.StepInfo))}");

        }
        return hb.ToString();
    }

    public async Task<string> RenderPartialViewAsync(DisplayStepsModule module, DisplayStepsModule.Model model) {

        HtmlBuilder hb = new HtmlBuilder();
        using (Manager.StartNestedComponent(nameof(model.StepInfo))) {

            hb.Append(await HtmlHelper.ForEditAsync(model, nameof(model.StepInfo)));

        }
        return hb.ToString();

    }
}
