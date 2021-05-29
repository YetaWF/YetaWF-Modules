/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.TawkTo.DataProvider;

namespace YetaWF.Modules.TawkTo.Addons {

    public class SkinTawkTo : IAddOnSupport {

        public async Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;

            ConfigData config = await ConfigDataProvider.GetConfigAsync();
            scripts.AddConfigOption(area, "ExcludedPagesCss", config.ExcludedPagesCss ?? string.Empty);
            scripts.AddConfigOption(area, "IncludedPagesCss", config.IncludedPagesCss ?? string.Empty);
        }
    }
}
