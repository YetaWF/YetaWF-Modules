/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.BootstrapCarousel.Addons.Templates {

    /// <summary>
    /// Template specific config strings.
    /// </summary>
    public class SlideShowEdit : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddLocalization(areaName, "RemoveConfirm", this.__ResStr("removeConfirm", "Are you sure you want to remove this image?"));
            scripts.AddLocalization(areaName, "RemoveTitle", this.__ResStr("removeTitle", "Remove Image"));

            return Task.CompletedTask;
        }
    }
}