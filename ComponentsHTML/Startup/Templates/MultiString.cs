/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class MultiString : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(area, "Localization", manager.CurrentSite.Localization);

            scripts.AddLocalization(area, "Languages", YetaWF.Core.Models.MultiString.LanguageIdList);
            scripts.AddLocalization(area, "NeedDefaultText", this.__ResStr("NeedDefaultText", "Please enter text for the default language before switching to another language."));

            return Task.CompletedTask;
        }
    }
}
