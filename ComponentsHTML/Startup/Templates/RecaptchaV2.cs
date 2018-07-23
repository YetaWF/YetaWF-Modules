/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class RecaptchaV2 : IAddOnSupport {

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
