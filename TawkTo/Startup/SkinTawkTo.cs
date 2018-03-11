/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.TawkTo.Controllers;
using YetaWF.Modules.TawkTo.DataProvider;

namespace YetaWF.Modules.TawkTo.Addons {

    public class SkinTawkTo : IAddOnSupport {

        public async Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;

            ConfigData config = await ConfigDataProvider.GetConfigAsync();
            //scripts.AddLocalization(area, "msg_expandSource", this.__ResStr("expandSource", "+ expand source"));
            scripts.AddConfigOption(area, "ExcludedPagesCss", config.ExcludedPagesCss);
            scripts.AddConfigOption(area, "IncludedPagesCss", config.IncludedPagesCss);
        }
    }
}
