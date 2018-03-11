/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.CurrencyConverter.Controllers;

namespace YetaWF.Modules.CurrencyConverter.Addons {

    public class Info : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddLocalization(areaName, "FmtResult", this.__ResStr("FmtResult", "{0} {1}"));

            return Task.CompletedTask;
        }
    }
}