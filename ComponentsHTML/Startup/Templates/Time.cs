/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class Time : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;
            scripts.AddVolatileOption(area, "TimeFormat", YetaWF.Core.Localize.Formatting.GetFormatTimeFormat());

            return Task.CompletedTask;
        }
    }
}
