/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Text.Addons {

    public class Info : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            return Task.CompletedTask;
        }
    }
}