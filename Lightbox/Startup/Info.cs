/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Lightbox#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Lightbox.Addons {

    public class Info : IAddOnSupport {

        /// <inheritdoc/>
        public Task AddSupportAsync(YetaWFManager manager) {
            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;
            scripts.AddLocalization(areaName, "ImageNumber", this.__ResStr("ImageNumber", "Image {0} of {1}"));
            return Task.CompletedTask;
        }
    }
}