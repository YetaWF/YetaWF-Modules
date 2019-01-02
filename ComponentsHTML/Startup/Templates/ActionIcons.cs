/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class ActionIcons : IAddOnSupport {

        public const string CssActionIcons = "yActionIcons"; // action icons

        public Task AddSupportAsync(YetaWFManager manager) {
            return Task.CompletedTask;
        }
    }
}
