/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/LoggingDataProvider#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.LoggingDataProvider.Addons {

    public class Info : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {
            return Task.CompletedTask;
        }
    }
}