/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Languages.Controllers;
using YetaWF.Modules.Languages.Modules;

namespace YetaWF.Modules.Languages.Views {

    public class LocalizeEditFileView : YetaWFView, IYetaWFView2<LocalizeEditFileModule, LocalizeEditFileModuleController.EditModel> {

        public const string ViewName = "LocalizeEditFile";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(LocalizeEditFileModule module, LocalizeEditFileModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            string btnReset = UniqueId();

            if (!LocalizationSupport.UseLocalizationResources) {
                hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {HE(this.__ResStr("disabled1", "Localization support is currently disabled - Use Localization Settings to enable."))}
</div>");
            } else if (!Manager.CurrentSite.Localization) {
                hb.Append($@"
<div class='{HE(Globals.CssDivWarning)}'>
    {HE(this.__ResStr("disabled2", "Localization support is currently disabled - Use Site Settings to enable."))}
</div>");
            }

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text=this.__ResStr("btnApply", "Apply") },
        new FormButton() { ButtonType= ButtonTypeEnum.Submit },
        new FormButton() { ButtonType= ButtonTypeEnum.Button, Text=this.__ResStr("btnReset", "Restore Defaults"), Title = this.__ResStr("btnResetTT", "Removes custom and installed localization resources for the current language (US-English default resources are never removed)"), Id = btnReset },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}
<script>
    $('#{btnReset}').on('click', function (e) {{
        var form = $YetaWF.Forms.getForm(this);
        $YetaWF.alertYesNo('{JE(this.__ResStr("confirmResetText","Are you sure you want to restore the default settings?"))}', null, function () {{
             $YetaWF.Forms.submit(form, true, 'RestoreDefaults=true');
        }});
    }});
</script>");

            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(LocalizeEditFileModule module, LocalizeEditFileModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();

        }
    }
}
