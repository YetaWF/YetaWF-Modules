/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Shows a captcha which prompts the user to confirm the he/she is not a robot by clicking on a checkbox.
    /// </summary>
    /// <example>
    /// [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
    /// [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot")]
    /// public RecaptchaV2Data Captcha { get; set; }
    /// </example>
    public class RecaptchaV2EditComponent : YetaWFComponent, IYetaWFComponent<RecaptchaV2Data> {

        internal const string TemplateName = "RecaptchaV2";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(RecaptchaV2Data model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"<div id='{DivId}' class='yt_recaptchav2'></div>");
            hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(RecaptchaV2Data.VerifyPresence)));

            if (Manager.IsPostRequest) { // We only need to render the recaptcha for postbacks. The initial setup is done in recaptchav2.js in response to the onload function called by google api

                Manager.ScriptManager.AddLast($@"
if (typeof grecaptcha != 'undefined') {{
    grecaptcha.render('{DivId}', {{
        'sitekey': YConfigs.YetaWF_ComponentsHTML.SiteKey,
        'theme': YConfigs.YetaWF_ComponentsHTML.Theme,
        'size': YConfigs.YetaWF_ComponentsHTML.Size,
    }});
}}");
            } else {
                Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.RecaptchaV2.recaptchaInit('{DivId}');");
            }
            return hb.ToString();
        }
    }
}
