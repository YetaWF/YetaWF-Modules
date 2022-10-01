/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.AddThis.Addons {

    public class Info : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {
            return Task.CompletedTask;
        }
    }
}