/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    /// <summary>
    /// Implements the YetaWF.Core.Addons.IAddOnSupport interface, which is called when the framework detects the use of the RecaptchaV2 component type.
    /// The AddSupportAsync method is called so RecaptchaV2 component specific configuration options and localizations can be added to the page.
    /// </summary>
    public class RecaptchaV2 : IAddOnSupport {

        /// <summary>
        /// Called by the framework so the component can add component specific client-side configuration options and localizations to the page.
        /// </summary>
        /// <param name="manager">The YetaWF.Core.Support.Manager instance of current HTTP request.</param>
        public async Task AddSupportAsync(YetaWFManager manager) {

            RecaptchaV2Config config = await RecaptchaV2Config.LoadRecaptchaV2ConfigAsync();
            if (string.IsNullOrWhiteSpace(config.PublicKey))
                throw new InternalError("The Recaptcha configuration settings are missing - no public key found");

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(area, "SiteKey", config.PublicKey);
            scripts.AddConfigOption(area, "Theme", config.GetTheme());
            scripts.AddConfigOption(area, "Size", config.GetSize());
        }
    }
}
