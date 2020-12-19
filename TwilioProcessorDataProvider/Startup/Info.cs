/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessorDataProvider#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.TwilioProcessorDataProvider.Addons {

    public class Info : IAddOnSupport {
        public Task AddSupportAsync(YetaWFManager manager) {
            return Task.CompletedTask;
        }
    }
}