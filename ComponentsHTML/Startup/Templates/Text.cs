/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class Text : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;

            string area = AreaRegistration.CurrentPackage.AreaName;
            scripts.AddLocalization(area, "CopyToClip", this.__ResStr("copyToClip", "Copied to clipboard"));

            return Task.CompletedTask;
        }
    }
}
