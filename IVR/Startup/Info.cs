/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.Addons {

    public class Info : IAddOnSupport {

        public const int ChSid = 10;
        public const int ChExtension = 6;

        public Task AddSupportAsync(YetaWFManager manager) {

            //ScriptManager scripts = manager.ScriptManager;
            //string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "something", Something);

            //scripts.AddLocalization(areaName, "something", this.__ResStr("something", "something"));

            return Task.CompletedTask;
        }
    }
}
