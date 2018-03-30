/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/FileSystem#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.FileSystem.Addons {

    public class Info : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            //ScriptManager scripts = manager.ScriptManager;
            //string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "something", Something);

            //scripts.AddLocalization(areaName, "something", this.__ResStr("something", "something"));

            return Task.CompletedTask;
        }
    }
}
