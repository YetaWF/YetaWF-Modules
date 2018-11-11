/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class RecaptchaV2EditComponent : YetaWFComponent, IYetaWFComponent<RecaptchaV2Data> {

        public const string TemplateName = "RecaptchaV2";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(RecaptchaV2Data model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"<div id='{DivId}' class='yt_recaptchav2'></div>");
            hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(RecaptchaV2Data.VerifyPresence)));

            if (Manager.IsPostRequest) { // We only need to render the recaptcha for postbacks. The initial setup is done in recaptchav2.js in response to the onload function called by google api
                hb.Append($@"
<script>
    if (typeof grecaptcha != 'undefined') {{
        grecaptcha.render('{DivId}', {{
            'sitekey': YConfigs.YetaWF_ComponentsHTML.SiteKey,
            'theme': YConfigs.YetaWF_ComponentsHTML.Theme,
            'size': YConfigs.YetaWF_ComponentsHTML.Size,
        }});
    }}
</script>");
            } else {
                hb.Append($@"
<script>
    new YetaWF_ComponentsHTML.RecaptchaV2.recaptchaInit('{DivId}');
</script>");
            }
            return hb.ToYHtmlString();
        }
    }
}
